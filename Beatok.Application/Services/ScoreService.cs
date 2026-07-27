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
                .FirstOrDefaultAsync(l => l.Id == lobbyId)
            ?? throw new NotFoundException("Lobby not found");
        
        if (lobby.Phase != LobbyPhase.Voting)
            throw new BadRequestException("Lobby is not in voting phase");

        var submission = await context.Submissions
            .Include(s => s.Participant)
            .Include(s => s.Scores)
            .FirstOrDefaultAsync(s => s.Id == dto.SubmissionId)
            ?? throw new NotFoundException("Submission not found");
        if (submission.Participant?.UserId == userId)
            throw new BadRequestException("User cannot vote for their own track");
        if (submission.Participant!.LobbyId != lobbyId)
            throw new BadRequestException("Submission is not part of this lobby");
        if (submission.Scores.Any(s => s.UserId == userId)) 
            throw new BadRequestException("User has already voted");

        var score = new Score
        {
            LobbyId = lobbyId,
            UserId = userId,
            SubmissionId = dto.SubmissionId,
            Value = dto.Value
        };
        
        await context.Scores.AddAsync(score);
        await context.SaveChangesAsync();
        await lobbyService.TryFinishVoting(lobby);
    }
}