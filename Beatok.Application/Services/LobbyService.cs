using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Category;
using Beatok.Application.DTOs.Lobby;
using Beatok.Application.DTOs.Sound;
using Beatok.Application.DTOs.Submission;
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
    ILobbyNotifier lobbyNotifier, IStorage storage, IKitService kitService) : ILobbyService
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

    public async Task StartAsync(Guid lobbyId, Guid userId)
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

        var kit = await kitService.GetRandomAsync();
        var categories = kit.Categories.Select(c => new RandomCategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Sounds = [.. c.Sounds.Select(s => new SoundDto
            {
                Id = s.Id,
                Value = storage.GeneratePresignedSoundUrl($"sounds/{s.Value}", TimeSpan.FromHours(1))
            })]
        }).ToList();
        await lobbyNotifier.StartedAsync(lobby.Id, categories);

        var jobId = backgroundJobClient.Schedule<ILobbyService>(
            s => s.TransitionToVotingAsync(lobby.Id),
            lobby.SubmissionTimeLimit);
        lobby.Phase = LobbyPhase.Submission;
        lobby.JobId = jobId;
        await unitOfWork.SaveChangesAsync();
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

        var submissions = lobby.Participants.SelectMany(p => p.Submissions.SelectMany(s => new List<SubmissionDto> {
            new() {
                Id = s.Id,
                Value = storage.GeneratePresignedSoundUrl($"sounds/{s.Value}", TimeSpan.FromHours(1)),
                User = new UserDto {
                    Id = s.Participant!.UserId,
                    Name = s.Participant!.User!.Name
                }
            }
        })).ToList();

        // TODO: Replace lobby job and add a background job to transition to the end phase after the voting time limit
        await lobbyNotifier.VotingStartedAsync(lobby.Id, submissions);
    }
}