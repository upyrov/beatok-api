using Beatok.Application.DTOs.Submission;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;
using Hangfire;

namespace Beatok.Application.Services;

public class SubmissionService(IUnitOfWork unitOfWork, IValidator<CreateSubmissionDto> createValidator,
    IValidator<UpdateSubmissionDto> updateValidator, IBackgroundJobClient backgroundJobClient,
    ILobbyNotifier lobbyNotifier, IStorage soundStorage) : ISubmissionService
{
    public async Task CreateAsync(CreateSubmissionDto dto, Guid userId)
    {
        var fluentValidationResult = await createValidator.ValidateAsync(dto);

        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }

        var lobby = await unitOfWork.Lobbies.GetByIdAsync(dto.LobbyId)
            ?? throw new NotFoundException("Lobby not found");

        if (lobby.Phase != LobbyPhase.Submission)
        {
            throw new InvalidOperationException("Lobby is not in submission phase");
        }

        var participation = lobby.Participants.FirstOrDefault(p => p.UserId == userId)
            ?? throw new InvalidOperationException("User is not a participant in this lobby");

        if (participation.Submissions != null && participation.Submissions.Count != 0)
        {
            throw new InvalidOperationException("User has already submitted a track");
        }

        var submission = new Submission
        {
            Value = dto.Value,
            ParticipantId = participation.Id
        };

        await unitOfWork.Submissions.CreateAsync(submission);

        // Initialize the collection if null and add the submission locally
        participation.Submissions ??= [];
        participation.Submissions.Add(submission);

        // Check if all connected participants have a submission
        if (lobby.Participants.All(p => p.IsConnected && p.Submissions != null && p.Submissions.Count != 0))
        {
            lobby.Phase = LobbyPhase.Voting;
            backgroundJobClient.Delete(lobby.JobId);
            lobbyNotifier.VotingStarted(lobby.Id, [.. lobby.Participants.SelectMany(p => p.Submissions)
                .Select(s => new SubmissionDto {
                    Id = s.Id,
                    Value = s.Value
                })]);
        }

        // TODO: Replace lobby job with a new job for the voting phase
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateValueAsync(Guid id, UpdateSubmissionDto dto)
    {
        var fluentValidationResult = await updateValidator.ValidateAsync(dto);
        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }

        var submission = await unitOfWork.Submissions.GetByIdAsync(id)
            ?? throw new NotFoundException("Submission not found");

        if (submission.Participant?.Lobby?.Phase != LobbyPhase.Submission)
        {
            throw new InvalidOperationException("Lobby is not in submission phase");
        }

        await unitOfWork.Submissions.UpdateValueAsync(submission.Id, dto.Value);
    }
}