using AutoMapper;
using Beatok.Application.DTOs.Score;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;

namespace Beatok.Application.Services;

public class ScoreService(IUnitOfWork unitOfWork, IValidator<CreateScoreDto> validator, 
    ILobbyService lobbyService, ILobbyNotifier lobbyNotifier, IMapper mapper)
    : IScoreService
{
    public async Task CreateAsync(Guid userId, Guid lobbyId, CreateScoreDto dto)
    {
        var fluentValidation = await validator.ValidateAsync(dto);
        if (!fluentValidation.IsValid)
        {
            throw new ValidationException(fluentValidation.Errors);
        }
        var lobby = await unitOfWork.Lobbies.GetByIdAsync(lobbyId);
        if (lobby == null)
            throw new NotFoundException("Lobby not found");
        
        if (lobby.Phase != LobbyPhase.Voting)
            throw new BadRequestException("Lobby is not in voting phase");

        var submission = await unitOfWork.Submissions.GetByIdAsync(dto.SubmissionId);
        if (submission == null)
            throw new NotFoundException("Submission not found");
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
        
        await unitOfWork.Scores.CreateAsync(score);
        await unitOfWork.SaveChangesAsync();
        await lobbyNotifier.VoteRegisteredAsync(mapper.Map<ScoreDto>(score));

        await lobbyService.TryFinishVoting(lobby);
    }
}