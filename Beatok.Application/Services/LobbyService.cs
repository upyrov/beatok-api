using AutoMapper;
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
using Microsoft.EntityFrameworkCore;

namespace Beatok.Application.Services;

public class LobbyService(IApplicationDbContext context,
    IValidator<CreateLobbyDto> validator, IBackgroundJobClient backgroundJobClient,
    ILobbyNotifier lobbyNotifier, IStorage storage, IKitService kitService,
    IMapper mapper, IMmrService mmrService) : ILobbyService
{
    public async Task<Guid> CreateAsync(CreateLobbyDto dto, Guid ownerId)
    {
        var fluentValidation = await validator.ValidateAsync(dto);
        if (!fluentValidation.IsValid)
        {
            throw new ValidationException(fluentValidation.Errors);
        }

        if (!await context.Users.AnyAsync(u => u.Id == ownerId))
            throw new NotFoundException("User not found");
        if (!await context.Genres.AnyAsync(g => g.Id == dto.GenreId))
            throw new NotFoundException("Genre not found");
        var activeLobbyCount = await context.Participation
            .Where(p => p.UserId == ownerId && p.Lobby!.State != LobbyState.Ended)
            .CountAsync();
        if (activeLobbyCount >= 2)
            throw new BadRequestException("User cannot join more than 2 active lobbies");

        var lobby = new Lobby
        {
            Name = dto.Name,
            OwnerId = ownerId,
            GenreId = dto.GenreId,
            ParticipantLimit = dto.ParticipantLimit,
            SubmissionTime = dto.SubmissionTime
        };

        await context.Lobbies.AddAsync(lobby);
        await context.SaveChangesAsync();
        return lobby.Id;
    }

    public async Task JoinAsync(Guid lobbyId, Guid userId)
    {
        var lobby = await context.Lobbies
                        .Include(l => l.Participants)
                        .FirstOrDefaultAsync(l => l.Id == lobbyId)
            ?? throw new NotFoundException("Lobby not found");
        var user = await context.Users.FindAsync(userId)
            ??throw new NotFoundException("User not found");

        var participant = lobby.Participants.FirstOrDefault(p =>
            p.UserId == user.Id && p.LobbyId == lobby.Id);
        if (participant != null)
        {
            await RejoinAsync(participant);
        }
        else
        {
            var activeLobbyCount = await context.Participation
                .Where(p => p.UserId == userId && p.Lobby!.State != LobbyState.Ended)
                .CountAsync();
            if (activeLobbyCount >= 2)
                throw new BadRequestException("User cannot join more than 2 active lobbies");
            if (lobby.State != LobbyState.Waiting)
                throw new BadRequestException("Lobby is already started");
            if (lobby.Participants.Count >= lobby.ParticipantLimit)
                throw new BadRequestException("Lobby is full");

            var newParticipant = new Participation
            {
                LobbyId = lobby.Id,
                UserId = user.Id
            };
            await context.Participation.AddAsync(newParticipant);
            await context.SaveChangesAsync();

            await lobbyNotifier.ParticipantJoinedAsync(lobby.Id, mapper.Map<ParticipationDto>(newParticipant));
        }
        if (lobby.ParticipantLimit == lobby.Participants.Count)
        {
            await StartAsync(lobbyId, lobby.OwnerId);
        }
    }

    private async Task RejoinAsync(Participation participant)
    {
        participant.IsConnected = true;
        await context.SaveChangesAsync();
        await lobbyNotifier.ParticipantConnectedAsync(participant.LobbyId, participant.UserId);
    }

    public async Task LeaveAsync(Guid lobbyId, Guid userId)
    {
        var lobby = await context.Lobbies
                        .Include(l => l.Participants)
                        .FirstOrDefaultAsync(l => l.Id == lobbyId)
            ?? throw new NotFoundException("Lobby not found");
        var user = await context.Users.FindAsync(userId)
            ?? throw new NotFoundException("User not found");
        var participant = lobby.Participants
            .FirstOrDefault(p => p.UserId == user.Id) ??
                          throw new NotFoundException("User not found in lobby");

        if (lobby.State == LobbyState.Waiting)
        {
            await HandleNotStartedLeaveAsync(lobby, participant);
            await lobbyNotifier.ParticipantLeftAsync(lobby.Id, userId);
        }
        else
        {
            await HandleStartedLeaveAsync(participant);
            await lobbyNotifier.ParticipantDisconnectedAsync(lobby.Id, userId);
        }
    }

    private async Task HandleNotStartedLeaveAsync(Lobby lobby, Participation participant)
    {
        var wasOwner = lobby.OwnerId == participant.UserId;
        
        lobby.Participants.Remove(participant);
        
        if (lobby.Participants.Count == 0)
        {
            context.Lobbies.Remove(lobby);
            await context.SaveChangesAsync();
            return;
        }

        if (wasOwner)
        {
            var newOwner = lobby.Participants
                .OrderBy(p => p.JoinedAt)
                .First();
            lobby.OwnerId = newOwner.UserId;
            await context.SaveChangesAsync();
            await lobbyNotifier.OwnerChangedAsync(lobby.Id, newOwner.UserId);
            return;
        }
        await context.SaveChangesAsync();
    }

    private async Task HandleStartedLeaveAsync(Participation participant)
    {
        participant.IsConnected = false;
        participant.ConnectionId = null;
        await context.SaveChangesAsync();
    }

    public async Task<LobbyWithParticipantsDto> SetConnectionIdAsync(Guid lobbyId, Guid userId, string connectionId)
    {
        var lobby = await context.Lobbies
                .Include(l => l.Participants)
                    .ThenInclude(p => p.User)
                .Include(l => l.Participants)
                .ThenInclude(p => p.Scores)
                .Include(l => l.Genre)
                .Include(l => l.Owner)
                .Include(l => l.Submissions)
                .Include(l => l.Sounds)
                    .ThenInclude(s => s.Category)
                .FirstOrDefaultAsync(l => l.Id == lobbyId)
            ?? throw new NotFoundException("Lobby not found");
        var participant = lobby.Participants
            .FirstOrDefault(p => p.UserId == userId) ??
                          throw new NotFoundException("User not found in lobby");
        
        participant.ConnectionId = connectionId;
        await context.SaveChangesAsync();

        foreach (var sound in lobby.Sounds)
        {
            sound.Value = storage.GeneratePresignedUrl(sound.Value, TimeSpan.FromHours(1));
        }

        foreach (var submission in lobby.Submissions)
        {
            submission.Value = storage.GeneratePresignedUrl(submission.Value, TimeSpan.FromHours(1));
        }
        return mapper.Map<LobbyWithParticipantsDto>(lobby);
    }

    public async Task DisconnectAsync(string connectionId)
    {
        var participations = await context.Participation
            .Include(p => p.Lobby)
                .ThenInclude(l => l!.Participants)
            .Where(p => p.ConnectionId == connectionId)
            .ToListAsync();
        
        foreach (var participation in participations)
        {
            if (participation.Lobby!.State == LobbyState.Waiting)
            {
                await HandleNotStartedLeaveAsync(participation.Lobby, participation);
                await lobbyNotifier.ParticipantLeftAsync(participation.LobbyId, participation.UserId);
            }
            else
            {
                await HandleStartedLeaveAsync(participation);
                await lobbyNotifier.ParticipantDisconnectedAsync(participation.LobbyId, participation.UserId);
            }
        }
    }
    
    public async Task<IEnumerable<LobbyDto>> GetAllAsync(LobbyFilterDto filter)
    {
        var query = context.Lobbies
            .Include(l => l.Genre)
            .Include(l => l.Owner)
            .Include(l => l.Participants)
            .Where(l => l.State == LobbyState.Waiting
                        && l.Participants.Count < l.ParticipantLimit);
        
        var lobbies = await ApplyFilter(query, filter).ToListAsync();
        
        return mapper.Map<IEnumerable<LobbyDto>>(lobbies);
    }
    
    private IQueryable<Lobby> ApplyFilter(IQueryable<Lobby> query, LobbyFilterDto filter)
    {
        if (!string.IsNullOrEmpty(filter.Name))
        {
            query = query.Where(l => l.Name.ToLower().Contains(filter.Name.ToLower().Trim()));
        }

        if (filter.GenreId.HasValue)
        {
            query = query.Where(l => l.GenreId == filter.GenreId);
        }

        return query;
    }

    public async Task StartAsync(Guid lobbyId, Guid userId)
    {
        var lobby = await context.Lobbies
                .Include(l => l.Participants)
                .FirstOrDefaultAsync(l => l.Id == lobbyId)
            ?? throw new NotFoundException("Lobby not found");
        if (lobby.OwnerId != userId)
            throw new BadRequestException("You are not the owner of this lobby");
        if (lobby.Participants.Count < 2)
            throw new BadRequestException("Lobby must have at least 2 participants");

        var kit = await kitService.GetRandomAsync();
        var sounds = kit.Categories.SelectMany(c => c.Sounds).ToList();
        lobby.Sounds = sounds;
        
        var soundsDto = sounds.Select(s => new SoundWithCategory
        {
            Id = s.Id,
            Value = storage.GeneratePresignedUrl(s.Value, TimeSpan.FromHours(1)),
            Category = new CategoryDto
            {
                Id = s.CategoryId,
                Name = s.Category!.Name
            }
        }).ToList();
        await lobbyNotifier.StartedAsync(lobby.Id, soundsDto);

        var jobId = backgroundJobClient.Schedule<ILobbyService>(
            s => s.TransitionToVotingAsync(lobby.Id),
            lobby.SubmissionTime);
        lobby.State = LobbyState.Submitting;
        lobby.SubmissionStartedAt = DateTime.UtcNow;
        lobby.JobId = jobId;
        await context.SaveChangesAsync();
    }

    public async Task TransitionToVotingAsync(Guid lobbyId)
    {
        var lobby = await context.Lobbies.
            Include(l => l.Participants)
                .ThenInclude(p => p.Submissions)
            .Where(l => l.Id == lobbyId)
            .FirstOrDefaultAsync();
        if (lobby == null)
            return;

        var submissions = lobby.Participants.SelectMany(p => p.Submissions.SelectMany(s => new List<SubmissionDto> {
            new() {
                Id = s.Id,
                Value = storage.GeneratePresignedUrl($"{s.Value}", TimeSpan.FromHours(1)),
                LobbyId = lobby.Id,
                ParticipationId = s.ParticipationId
            }
        })).ToList();

        if (submissions.Count == 0)
        {
            await TransitionToEndAsync(lobby.Id);
            return;
        }
        
        var votingTime = TimeSpan.FromSeconds(lobby.Participants
            .SelectMany(s => s.Submissions)
            .Sum(s => s.DurationSeconds)) + TimeSpan.FromMinutes(1);
        var jobId = backgroundJobClient.Schedule<ILobbyService>(
            s => s.TransitionToEndAsync(lobby.Id), votingTime);
        
        lobby.State = LobbyState.Voting;
        lobby.VotingStartedAt = DateTime.UtcNow;
        lobby.VotingTime = votingTime;
        lobby.JobId = jobId;
        await context.SaveChangesAsync();
        
        await lobbyNotifier.VotingStartedAsync(lobby.Id, votingTime, submissions);
    }

    public async Task TryFinishVoting(Lobby lobby)
    {
        var submissions = lobby.Submissions.ToList();
        
        var scores = submissions.SelectMany(s => s.Scores).ToList();
        
        var expectedVotes = lobby.Participants.Sum(participant =>
            submissions.Count(s => s.ParticipationId != participant.Id));
        if (scores.Count != expectedVotes)
            return;

        backgroundJobClient.Delete(lobby.JobId);
        await TransitionToEndAsync(lobby.Id);
    }

    public async Task TransitionToEndAsync(Guid lobbyId)
    {
        var lobby = await context.Lobbies
            .Include(l => l.Participants)
                .ThenInclude(p => p.Submissions)
                    .ThenInclude(s => s.Scores)
            .Include(l => l.Participants)
                .ThenInclude(p => p.User)
            .Where(l => l.Id == lobbyId)
            .FirstOrDefaultAsync();
        if (lobby == null)
            return;

        lobby.State = LobbyState.Ended;
        lobby.EndedAt = DateTime.UtcNow;
        
        var winnerSubmission = GetWinnerSubmission(lobby);
        
        var ratingResults = mmrService.CalculateRatings(lobby);

        if (!ratingResults.Any())
            return;

        foreach (var participant in lobby.Participants)
        {
            if (participant.User != null && ratingResults.TryGetValue(participant.UserId, out var result))
            {
                participant.User.Mu = result.NewMu;
                participant.User.Sigma = result.NewSigma;
            }
        }

        if (winnerSubmission != null)
        {
            lobby.WinningSubmissionId = winnerSubmission.Id;
        }
        await context.SaveChangesAsync();

        var ratingChanges = ratingResults
            .Select(r => new UserRatingChangeDto
            {
                UserId = r.Key,
                RatingChange = r.Value.RatingChange,
            }).ToList();
        
        await lobbyNotifier.EndedAsync(lobby.Id, winnerSubmission.Id, ratingChanges); 
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

    public async Task SendMessageAsync(Guid lobbyId, Guid userId, string content)
    {
        var lobby = await context.Lobbies
                .Include(l => l.Participants)
                .FirstOrDefaultAsync(l => l.Id == lobbyId)
            ?? throw new NotFoundException("Lobby not found");
        var participant = lobby.Participants
            .FirstOrDefault(p => p.UserId == userId) ??
                          throw new NotFoundException("User not found in lobby");
        
        if (string.IsNullOrWhiteSpace(content))
            throw new BadRequestException("Message cannot be empty");
        if (content.Length > 250)
            throw new BadRequestException("Message cannot be longer than 250 characters");

        await lobbyNotifier.MessageReceivedAsync(lobbyId, userId, content);
    }
}