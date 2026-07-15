using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Category;
using Beatok.Application.DTOs.Lobby;
using Beatok.Application.DTOs.Sound;
using Beatok.Application.DTOs.User;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;
using Hangfire;

namespace Beatok.Application.Services;

public class LobbyService(IUnitOfWork unitOfWork,
    IValidator<CreateLobbyDto> validator, IBackgroundJobClient backgroundJobClient,
    ILobbyNotifier lobbyNotifier, ISoundStorage soundStorage, IKitService kitService) : ILobbyService
{
    public async Task CreateAsync(CreateLobbyDto dto, Guid ownerId)
    {
        var fluentValidation = await validator.ValidateAsync(dto);
        if (!fluentValidation.IsValid)
        {
            throw new ValidationException(fluentValidation.Errors);
        }

        var owner = await unitOfWork.Users.GetByIdAsync(ownerId)
            ?? throw new NotFoundException("User not found");
        var genre = await unitOfWork.Genres.GetByIdAsync(dto.GenreId)
            ?? throw new NotFoundException("Genre not found");
        var lobby = new Lobby
        {
            Name = dto.Name,
            OwnerId = owner.Id,
            GenreId = genre.Id,
            ParticipantLimit = dto.ParticipantLimit,
            SubmissionTimeLimit = dto.SubmissionTimeLimit,
            VotingTimeLimit = dto.VotingTimeLimit
        };

        await unitOfWork.Lobbies.AddAsync(lobby);

        await unitOfWork.Participations.AddAsync(new Participation
        {
            LobbyId = lobby.Id,
            UserId = owner.Id
        });
        await unitOfWork.SaveChangesAsync();
    }

    public async Task JoinAsync(Guid lobbyId, Guid userId)
    {
        var lobby = await unitOfWork.Lobbies.GetByIdAsync(lobbyId)
            ?? throw new NotFoundException("Lobby not found");
        var user = await unitOfWork.Users.GetByIdAsync(userId)
            ??throw new NotFoundException("User not found");

        var participant = lobby.Participants.FirstOrDefault(p =>
            p.UserId == user.Id && p.LobbyId == lobby.Id);
        if (participant != null)
        {
            await RejoinAsync(user, lobby, participant);
        }
        else
        {
            if (lobby.Phase != LobbyPhase.NotStarted)
                throw new BadRequestException("Lobby is already started");
            if (lobby.Participants.Count >= lobby.ParticipantLimit)
                throw new BadRequestException("Lobby is full");

            var newParticipant = new Participation
            {
                LobbyId = lobby.Id,
                UserId = user.Id
            };
            await unitOfWork.Participations.AddAsync(newParticipant);
            await unitOfWork.SaveChangesAsync();

            await lobbyNotifier.ParticipantJoinedAsync(lobby.Id, new UserDto
            {
                Name = user.Name
            });
        }
    }

    private async Task RejoinAsync(User user, Lobby lobby, Participation participant)
    {
        participant.IsConnected = true;
        await unitOfWork.SaveChangesAsync();
        await lobbyNotifier.ParticipantRejoinedAsync(lobby.Id, new UserDto
        {
            Name = user.Name
        });
    }

    public async Task<IEnumerable<LobbyDto>> GetAllAsync(LobbyFilterDto filter)
    {
        var lobbies = await unitOfWork.Lobbies.GetFilteredAsync(filter);
        return lobbies.Select(l => new LobbyDto
        {
            Id = l.Id,
            Name = l.Name,
            CreatedAt = l.CreatedAt,
            GenreId = l.GenreId,
            ParticipantLimit = l.ParticipantLimit,
            SubmissionTimeLimit = l.SubmissionTimeLimit,
            VotingTimeLimit = l.VotingTimeLimit,
            Phase = l.Phase,
            OwnerId = l.OwnerId
        }
        );
    }

    public async Task StartLobbyAsync(Guid lobbyId, Guid userId)
    {
        var lobby = await unitOfWork.Lobbies.GetByIdAsync(lobbyId)
            ?? throw new NotFoundException("Lobby not found");
        if (lobby.OwnerId != userId)
        {
            throw new BadRequestException("You are not the owner of this lobby");
        }
        if (lobby.Participants.Count < 2)
        {
            throw new BadRequestException("Lobby must have at least 2 participants");
        }

        lobby.Phase = LobbyPhase.Submission;
        await unitOfWork.SaveChangesAsync();

        var kit = await kitService.GetRandomAsync();
        var categories = kit.Categories.Select(c => new RandomCategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Sounds = [.. c.Sounds.Select(s => new SoundDto
            {
                Id = s.Id,
                Value = soundStorage.GeneratePresignedSoundUrl($"sounds/{s.Value}", TimeSpan.FromHours(1))
            })]
        }).ToList();
        await lobbyNotifier.StartedAsync(lobby.Id, categories);

        backgroundJobClient.Schedule<ILobbyService>(
            s => s.TransitionToVotingAsync(lobby.Id),
            lobby.SubmissionTimeLimit);
    }

    public async Task TransitionToVotingAsync(Guid lobbyId)
    {
        var lobby = await unitOfWork.Lobbies.GetByIdAsync(lobbyId);
        if (lobby == null)
        {
            return;
        }

        lobby.Phase = LobbyPhase.Voting;
        await unitOfWork.SaveChangesAsync();

        // TODO: [Stub] Currently returning an empty list.
        // Needs to be integrated with a FileStorageService to fetch submission URLs from the bucket.
        var submissions = new List<string>();

        await lobbyNotifier.VotingStartedAsync(lobby.Id, submissions);
    }
}