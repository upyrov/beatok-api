using AutoMapper;
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
    IStorage storage, ILobbyService lobbyService) : ISubmissionService
{
    public SubmissionUploadDto GenerateUploadUrl(string fileExtension, string contentType)
    {
        if (!fileExtension.StartsWith('.'))
        {
            fileExtension = $".{fileExtension}";
        }

        var fileKey = $"submissions/{Guid.NewGuid()}{fileExtension}";
        var uploadUrl = storage.GeneratePresignedUploadUrl(fileKey, TimeSpan.FromMinutes(15), contentType);

        return new SubmissionUploadDto
        {
            UploadUrl = uploadUrl,
            FileKey = fileKey
        };
    }

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

        if (dto.DurationSeconds <= 0 || dto.DurationSeconds > lobby.SubmissionTimeLimit.TotalSeconds / 2)
        {
            throw new ValidationException("Duration seconds must be a positive value and not exceed half the submission time limit");
        }

        var submission = new Submission
        {
            Value = dto.Value,
            ParticipantId = participation.Id,
            DurationSeconds = dto.DurationSeconds
        };

        await unitOfWork.Submissions.CreateAsync(submission);

        // Initialize the collection if null and add the submission locally
        participation.Submissions ??= [];

        // Check if all connected participants have a submission
        if (lobby.Participants.Where(p => p.IsConnected).All(p => p.Submissions.Count != 0))
        {
            backgroundJobClient.Delete(lobby.JobId);
            await lobbyService.TransitionToVotingAsync(lobby.Id);
        }
        else
        {
            await unitOfWork.SaveChangesAsync();
        }
    }

    public async Task UpdateValueAsync(Guid id, UpdateSubmissionDto dto, Guid userId)
    {
        var fluentValidationResult = await updateValidator.ValidateAsync(dto);
        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }

        var submission = await unitOfWork.Submissions.GetByIdAsync(id)
            ?? throw new NotFoundException("Submission not found");

        if (submission.Participant?.UserId != userId)
        {
            throw new UnauthorizedAccessException("User is not the owner of this submission");
        }

        if (submission.Participant?.Lobby?.Phase != LobbyPhase.Submission)
        {
            throw new InvalidOperationException("Lobby is not in submission phase");
        }

        await unitOfWork.Submissions.UpdateValueAsync(submission.Id, dto.Value);
    }
}