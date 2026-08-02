using Beatok.Application.DTOs.Score;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Application.Services;

public class ScoreService(IApplicationDbContext context, IValidator<CreateScoreDto> validator, 
    ILobbyService lobbyService) : IScoreService
{
    public async Task CreateAsync(Guid userId, Guid lobbyId, CreateScoreDto dto)
    {
        var fluentValidation = await validator.ValidateAsync(dto);
        if (!fluentValidation.IsValid)
        {
            throw new ValidationException(fluentValidation.Errors);
        }
        var lobby = await context.Lobbies
                .Include(l => l.Participants)
                .ThenInclude(p => p.Submissions)
                .ThenInclude(s => s.Scores)
                .FirstOrDefaultAsync(l => l.Id == lobbyId)
            ?? throw new NotFoundException("Lobby not found");
        
        if (lobby.State != LobbyState.Voting)
            throw new BadRequestException("Lobby is not in voting phase");

        var submission = await context.Submissions
            .Include(s => s.Participant)
            .Include(s => s.Scores)
            .FirstOrDefaultAsync(s => s.Id == dto.SubmissionId)
            ?? throw new NotFoundException("Submission not found");
        
        var participation = lobby.Participants
            .FirstOrDefault(p => p.UserId == userId);
        if (participation == null)
        {
            throw new BadRequestException("User is not a participant in this lobby");
        }
        
        if (submission.Participant?.UserId == userId)
            throw new BadRequestException("User cannot vote for their own track");
        if (submission.Participant!.LobbyId != lobbyId)
            throw new BadRequestException("Submission is not part of this lobby");
        if (submission.Scores.Any(s => s.ParticipationId == participation.Id)) 
            throw new BadRequestException("User has already voted");

        var score = new Score
        {
            LobbyId = lobbyId,
            ParticipationId = participation.Id,
            SubmissionId = dto.SubmissionId,
            Value = dto.Value
        };
        
        await context.Scores.AddAsync(score);
        await context.SaveChangesAsync();
        await lobbyService.TryFinishVoting(lobby);
    }

    public async Task UpdateValueAsync(Guid id, UpdateScoreDto dto)
    {
        var score = await context.Scores.FindAsync(id) ?? throw new NotFoundException("Score not found");
        score.Value = dto.Value;
        context.Scores.Update(score);
        await context.SaveChangesAsync();
    }
}