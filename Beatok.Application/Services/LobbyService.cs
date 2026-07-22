using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Category;
using Beatok.Application.DTOs.Genre;
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
    public async Task<Guid> CreateAsync(CreateLobbyDto dto, Guid ownerId)
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
        var activeLobbyCount = await unitOfWork.Participations.CountActiveByUserIdAsync(ownerId);
        if (activeLobbyCount >= 2)
            throw new BadRequestException("User cannot join more than 2 active lobbies");

        var lobby = new Lobby
        {
            Name = dto.Name,
            OwnerId = owner.Id,
            GenreId = genre.Id,
            ParticipantLimit = dto.ParticipantLimit,
            SubmissionTimeLimit = dto.SubmissionTimeLimit
        };

        await unitOfWork.Lobbies.AddAsync(lobby);

        await unitOfWork.Participations.AddAsync(new Participation
        {
            LobbyId = lobby.Id,
            UserId = owner.Id
        });
        await unitOfWork.SaveChangesAsync();

        return lobby.Id;
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
            var activeLobbyCount = await unitOfWork.Participations.CountActiveByUserIdAsync(userId);
            if (activeLobbyCount >= 2)
                throw new BadRequestException("User cannot join more than 2 active lobbies");
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
                Id = user.Id,
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
            Id = user.Id,
            Name = user.Name
        });
    }

    public async Task LeaveAsync(Guid lobbyId, Guid userId)
    {
        var lobby = await unitOfWork.Lobbies.GetByIdAsync(lobbyId)
            ?? throw new NotFoundException("Lobby not found");
        var user = await unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found");
        var participant = lobby.Participants
            .FirstOrDefault(p => p.UserId == user.Id) ??
                          throw new NotFoundException("User not found in lobby");

        if (lobby.Phase == LobbyPhase.NotStarted)
        {
            await HandleNotStartedLeaveAsync(lobby, participant);
        }
        else
        {
            await HandleStartedLeaveAsync(participant);
        }
        await lobbyNotifier.ParticipantLeftAsync(lobby.Id, new UserDto
        {
            Id = user.Id,
            Name = user.Name
        });
    }

    private async Task HandleNotStartedLeaveAsync(Lobby lobby, Participation participant)
    {
        var wasOwner = lobby.OwnerId == participant.UserId;
        
        lobby.Participants.Remove(participant);
        unitOfWork.Participations.Delete(participant);
        
        if (lobby.Participants.Count == 0)
        {
            unitOfWork.Lobbies.Delete(lobby);
            await unitOfWork.SaveChangesAsync();
            return;
        }

        if (wasOwner)
        {
            var newOwner = lobby.Participants
                .OrderBy(p => p.JoinedAt)
                .First();
            lobby.OwnerId = newOwner.UserId;
            await unitOfWork.SaveChangesAsync();
            await lobbyNotifier.OwnerChangedAsync(lobby.Id, newOwner.UserId);
            return;
        }
        await unitOfWork.SaveChangesAsync();
    }

    private async Task HandleStartedLeaveAsync(Participation participant)
    {
        participant.IsConnected = false;
        participant.ConnectionId = null;
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<LobbyWithParticipantsDto> SetConnectionIdAsync(Guid lobbyId, Guid userId, string connectionId)
    {
        var lobby = await unitOfWork.Lobbies.GetByIdAsync(lobbyId)
            ?? throw new NotFoundException("Lobby not found");
        var participant = lobby.Participants
            .FirstOrDefault(p => p.UserId == userId) ??
                          throw new NotFoundException("User not found in lobby");
        
        participant.ConnectionId = connectionId;
        await unitOfWork.SaveChangesAsync();

        return new LobbyWithParticipantsDto
        {
            Id = lobby.Id,
            Name = lobby.Name,
            CreatedAt = lobby.CreatedAt,
            Genre = new GenreDto
            {
                Id = lobby.Genre!.Id,
                Name = lobby.Genre.Name
            },
            Owner = new UserDto
            {
                Id = lobby.Owner!.Id,
                Name = lobby.Owner.Name
            },
            ParticipantLimit = lobby.ParticipantLimit,
            Participants = [.. lobby.Participants.Select(p => new UserDto
            {
                Id = p.UserId,
                Name = p.User!.Name
            })],
            SubmissionTimeLimit = lobby.SubmissionTimeLimit
        };
    }

    public async Task DisconnectAsync(string connectionId)
    {
        var participations = await unitOfWork.Participations
            .GetByConnectionIdAsync(connectionId);
        
        foreach (var participation in participations)
        {
            if (participation.Lobby!.Phase == LobbyPhase.NotStarted)
            {
                await HandleNotStartedLeaveAsync(participation.Lobby, participation);
            }
            else
            {
                await HandleStartedLeaveAsync(participation);
            }

            await lobbyNotifier.ParticipantLeftAsync(participation.LobbyId, new UserDto
            {
                Name = participation.User!.Name,
                Id = participation.User.Id
            });
        }
    }
    
    public async Task<IEnumerable<LobbyDto>> GetAllAsync(LobbyFilterDto filter)
    {
        var lobbies = await unitOfWork.Lobbies.GetFilteredAsync(filter);
        return lobbies.Select(l => new LobbyDto
            {
                Id = l.Id,
                Name = l.Name,
                CreatedAt = l.CreatedAt,
                Genre = new GenreDto
                {
                    Id = l.Genre!.Id,
                    Name = l.Genre.Name
                },
                Owner = new UserDto
                {
                    Id = l.Owner!.Id,
                    Name = l.Owner.Name
                },
                ParticipantLimit = l.ParticipantLimit,
                ParticipantCount = l.Participants.Count,
                SubmissionTimeLimit = l.SubmissionTimeLimit
            }
        );
    }

    public async Task StartAsync(Guid lobbyId, Guid userId)
    {
        var lobby = await unitOfWork.Lobbies.GetByIdAsync(lobbyId)
            ?? throw new NotFoundException("Lobby not found");
        if (lobby.OwnerId != userId)
            throw new BadRequestException("You are not the owner of this lobby");
        if (lobby.Participants.Count < 2)
            throw new BadRequestException("Lobby must have at least 2 participants");

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

        var submissions = lobby.Participants.SelectMany(p => p.Submissions.SelectMany(s => new List<SubmissionDto> {
            new() {
                Id = s.Id,
                Value = s.Value,
                LobbyId = lobby.Id,
                User = new UserDto {
                    Id = s.Participant!.UserId,
                    Name = s.Participant!.User!.Name
                }
            }
        })).ToList();
        
        var votingTime = TimeSpan.FromSeconds(lobby.Participants
            .SelectMany(s => s.Submissions)
            .Sum(s => s.DurationSeconds)) + TimeSpan.FromMinutes(1);
        var jobId = backgroundJobClient.Schedule<LobbyService>(
            s => s.TransitionToEndAsync(lobby.Id), votingTime);
        
        lobby.Phase = LobbyPhase.Voting;
        lobby.JobId = jobId;
        await unitOfWork.SaveChangesAsync();
        
        await lobbyNotifier.VotingStartedAsync(lobby.Id, submissions);
    }

    public async Task TryFinishVoting(Lobby lobby)
    {
        var submissions = lobby.Participants.SelectMany(p => p.Submissions).ToList();
        
        var scores = submissions.SelectMany(s => s.Scores).ToList();
        
        var expectedVotes = lobby.Participants.Sum(participant =>
            submissions.Count(s => s.Participant!.UserId != participant.UserId));
        if (scores.Count != expectedVotes)
            return;

        backgroundJobClient.Delete(lobby.JobId);
        await TransitionToEndAsync(lobby.Id);
    }

    public async Task TransitionToEndAsync(Guid lobbyId)
    {
        var lobby = await unitOfWork.Lobbies.GetByIdAsync(lobbyId);
        if (lobby == null)
            return;

        lobby.Phase = LobbyPhase.End;
        await unitOfWork.SaveChangesAsync();
        
        var winnerSubmission = GetWinnerSubmission(lobby);

        if (winnerSubmission == null)
        {
            await lobbyNotifier.EndedAsync(null, null, lobby.Id);
            return;   
        }
        
        var winnerUserDto = new UserDto
        {
            Id = winnerSubmission.Participant!.UserId,
            Name = winnerSubmission.Participant!.User!.Name
        };

        var winnerSubmissionDto = new SubmissionDto
        {
            Id = winnerSubmission.Id,
            Value = storage.GeneratePresignedSoundUrl($"submissions/{winnerSubmission.Value}", TimeSpan.FromHours(1)),
            User = winnerUserDto
        };
        await lobbyNotifier.EndedAsync(winnerUserDto, winnerSubmissionDto, lobby.Id); 
    }

    private Submission? GetWinnerSubmission(Lobby lobby)
    {
        var submissions = lobby.Participants.SelectMany(p => p.Submissions).ToList();
        if (!submissions.Any(s => s.Scores.Any()))
            return null;

        var totalScore = submissions.Select(s => new
        {
            Submission = s,
            TotalScore = s.Scores.Sum(x => x.Value),
            LastVote = s.Scores.Max(x => x.CreatedAt)
        });
        
        var winner = totalScore
            .OrderByDescending(s => s.TotalScore)
            .ThenBy(s => s.LastVote)
            .First();
        
        return winner.Submission;
    }
}