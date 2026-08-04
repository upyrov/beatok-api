using Beatok.Application.DTOs.Score;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Application.Services;

public class ScoreService(IApplicationDbContext context, IValidator<CreateScoreDto> createValidator, 
    IValidator<ScoreUpdateDto> updateValidator, ILobbyService lobbyService) : IScoreService
{
    public async Task<Guid> CreateAsync(Guid userId, Guid lobbyId, CreateScoreDto dto)
    {
        var fluentValidation = await createValidator.ValidateAsync(dto);
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
            .FirstOrDefault(p => p.UserId == userId && !p.IsKicked) ?? throw new BadRequestException("User is not a participant in this lobby");
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
        await lobbyService.TryFinishVotingAsync(lobby);
        return score.Id;
    }

    public async Task UpdateValueAsync(Guid userId, Guid lobbyId, Guid scoreId, ScoreUpdateDto dto)
    {
        var fluentValidation = await updateValidator.ValidateAsync(dto);
        if (!fluentValidation.IsValid)
        {
            throw new ValidationException(fluentValidation.Errors);
        }
        var lobby = await context.Lobbies.FirstOrDefaultAsync(l => l.Id == lobbyId)
            ?? throw new NotFoundException("Lobby not found");

        if (lobby.State != LobbyState.Voting)
            throw new BadRequestException("Lobby is not in voting phase");

        var score = await context.Scores.Include(s => s.Participant).FirstOrDefaultAsync(s => s.Id == scoreId)
            ?? throw new NotFoundException("Score not found");

        if (score.LobbyId != lobbyId)
            throw new BadRequestException("Score is not part of this lobby");
        
        if (score.Participant?.UserId != userId)
            throw new BadRequestException("User is not the owner of this score");

        score.Value = dto.Value;
        context.Scores.Update(score);
        await context.SaveChangesAsync();
    }
}