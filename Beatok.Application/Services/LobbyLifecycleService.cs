using Beatok.Application.DTOs.Category;
using Beatok.Application.DTOs.Sound;
using Beatok.Application.DTOs.Submission;
using Beatok.Application.DTOs.User;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Application.Services;

public class LobbyLifecycleService(IApplicationDbContext context, 
    IKitService kitService,
    IStorage storage,
    ILobbyNotifier lobbyNotifier,
    IBackgroundJobClient backgroundJobClient,
    IMmrService mmrService): ILobbyLifecycleService
{
    public async Task StartAsync(Guid lobbyId, string userId)
    {
        var lobby = await context.Lobbies
                        .Include(l => l.Participants)
                        .FirstOrDefaultAsync(l => l.Id == lobbyId)
                    ?? throw new NotFoundException("Lobby not found");
        if (lobby.OwnerId != userId)
            throw new BusinessException("You are not the owner of this lobby");
        if (lobby.Participants.Count(p => !p.IsKicked) < 2)
            throw new BusinessException("Lobby must have at least 2 participants");
        if (lobby.State != LobbyState.Waiting)
            return;

        var soundIds = await kitService.GetRandomSoundIdsAsync(lobby.GenreId);

        var sounds = await context.Sounds
            .Include(s => s.Category)
            .Where(s => soundIds.Contains(s.Id))
            .ToListAsync();

        lobby.Sounds = sounds;
        
        var soundsDto = sounds.Select(s => new SoundWithCategory
        {
            Id = s.Id,
            Value = storage.GeneratePresignedUrl($"sounds/{s.Value}", TimeSpan.FromHours(1)),
            Name = s.Name,
            Category = new CategoryDto
            {
                Id = s.CategoryId,
                Name = s.Category!.Name
            }
        }).ToList();
        await lobbyNotifier.StartedAsync(lobby.Id, soundsDto);

        var jobId = backgroundJobClient.Schedule<ILobbyLifecycleService>(
            s => s.TransitionToVotingAsync(lobby.Id),
            lobby.SubmissionTime);
        lobby.State = LobbyState.Submitting;
        lobby.SubmissionStartedAt = DateTime.UtcNow;
        lobby.JobId = jobId;
        await context.SaveChangesAsync();
    }
    
    public async Task TransitionToVotingAsync(Guid lobbyId)
    {
        var lobby = await context.Lobbies.FindAsync(lobbyId);
        if (lobby == null)
            return;

        var hasSubmissions = await context.Submissions
            .AnyAsync(s =>
                s.LobbyId == lobbyId &&
                !s.Participant!.IsKicked);

        if (!hasSubmissions)
        {
            await TransitionToEndAsync(lobby.Id);
            return;
        }
        
        lobby.State = LobbyState.Voting;
        lobby.VotingStartedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        
        await lobbyNotifier.VotingStartedAsync(lobby.Id);
        await StartPlaybackAsync(lobbyId);
    }
    
    public async Task StartPlaybackAsync(Guid lobbyId)
    {
        var submissions = await context.Submissions
            .Where(s =>
                s.LobbyId == lobbyId &&
                !s.Participant!.IsKicked)
            .ToListAsync();

        for (int i = 0; i < submissions.Count; i++)
        {
            await context.LobbyPlaybackItems.AddAsync(new LobbyPlaybackItem
            {
                LobbyId = lobbyId,
                SubmissionId = submissions[i].Id,
                Order = i
            });
        }
        await context.SaveChangesAsync();
        
        await PlayNextItemAsync(lobbyId, 0);
    }
    
    public async Task PlayNextItemAsync(Guid lobbyId, int order)
    {
        var item = await context.LobbyPlaybackItems
            .Include(x => x.Submission)
            .FirstOrDefaultAsync(x =>
                x.LobbyId == lobbyId &&
                x.Order == order);
        
        if (item is null)
        {
            await TransitionToEndAsync(lobbyId);
            return;
        }
        
        var startedAt = DateTime.UtcNow;
        item.StartedAt = startedAt;
        await context.SaveChangesAsync();
        
        await lobbyNotifier.SubmissionForPlaybackAsync(lobbyId, 
            new SubmissionDto
            {
                Id = item.SubmissionId,
                LobbyId = lobbyId,
                ParticipationId = item.Submission!.ParticipationId,
                Value = storage.GeneratePresignedUrl($"submissions/{item.Submission.Value}", TimeSpan.FromHours(1))
            }, 
            startedAt);
        
        backgroundJobClient.Schedule<ILobbyLifecycleService>(
            s => s.PlayNextItemAsync(lobbyId, item.Order + 1),
            TimeSpan.FromSeconds(item.Submission!.DurationSeconds));
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

        var ratingChanges = new List<RatingChangeDto>();
        
        foreach (var participant in lobby.Participants.Where(p => !p.IsKicked))
        {
            if (participant.User != null && ratingResults.TryGetValue(participant.UserId, out var result))
            {
                participant.User.Mu = result.NewMu;
                participant.User.Sigma = result.NewSigma;

                var ratingDelta = result.RatingChange >= 0
                    ? (int)Math.Round(result.RatingChange * 10)
                    : (int)Math.Round(result.RatingChange * 5);
                participant.User.Rating = Math.Max(0, participant.User.Rating + ratingDelta);
                
                ratingChanges.Add(new RatingChangeDto
                {
                    UserId = participant.UserId,
                    RatingChange = ratingDelta
                });
            }
        }

        if (winnerSubmission != null)
        {
            lobby.WinningSubmissionId = winnerSubmission.Id;
        }
        await context.SaveChangesAsync();
        
        await lobbyNotifier.EndedAsync(lobby.Id, winnerSubmission?.Id, ratingChanges); 
    }
    
    private Submission? GetWinnerSubmission(Lobby lobby)
    {
        var submissions = lobby.Participants
            .Where(p => !p.IsKicked)
            .SelectMany(p => p.Submissions)
            .Where(s => s.Scores.Any())
            .ToList();
        
        if (!submissions.Any())
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