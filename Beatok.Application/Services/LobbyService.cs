using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Lobby;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;
using Hangfire;

namespace Beatok.Application.Services;

public class LobbyService(IUnitOfWork unitOfWork,
    IValidator<CreateLobbyDto> validator, IBackgroundJobClient backgroundJobClient,
    ILobbyNotifier lobbyNotifier): ILobbyService
{
    public async Task CreateAsync(CreateLobbyDto dto, Guid ownerId)
    {
        var fluentValidation = await validator.ValidateAsync(dto);
        if (!fluentValidation.IsValid)
        {
            throw new ValidationException(fluentValidation.Errors);
        }

        var owner = await unitOfWork.Users.GetByIdAsync(ownerId);

        if (owner == null)
        {
            throw new NotFoundException("User not found");
        }
        
        var genre = await unitOfWork.Genres.GetByIdAsync(dto.GenreId);
        if (genre == null)
        {
            throw new NotFoundException("Genre not found");       
        }

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

        await unitOfWork.Participation.AddAsync(new Participation
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

    public async Task StartLobbyAsync(Guid lobbyId, Guid userId)
    {
        var lobby = await unitOfWork.Lobbies.GetByIdAsync(lobbyId);
        if (lobby == null)
        {
            throw new NotFoundException("Lobby not found");
        }

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

        // TODO: [Stub] Replace with actual SoundKit generation service call
        var kit = new List<string>();
        
        lobbyNotifier.Started(lobby.Id, kit);

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
        
        lobbyNotifier.VotingStarted(lobby.Id, submissions);
    }
}