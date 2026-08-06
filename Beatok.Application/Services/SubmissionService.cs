using Beatok.Application.DTOs.Submission;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Application.Services;

public class SubmissionService(IApplicationDbContext context, IValidator<CreateSubmissionDto> createValidator,
    IValidator<SubmissionUpdateDto> updateValidator, IBackgroundJobClient backgroundJobClient,
    IStorage storage, ILobbyService lobbyService) : ISubmissionService
{
    public SubmissionUploadDto GenerateUploadUrl(string fileExtension, string contentType)
    {
        if (!fileExtension.StartsWith('.'))
        {
            fileExtension = $".{fileExtension}";
        }

        var fileKey = $"{Guid.NewGuid()}{fileExtension}";
        var uploadUrl = storage.GeneratePresignedUploadUrl($"submissions/{fileKey}", TimeSpan.FromMinutes(15), contentType);

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

        var lobby = await context.Lobbies
                .Include(l => l.Participants)
                    .ThenInclude(p => p.Submissions)
                .FirstOrDefaultAsync(l => l.Id == dto.LobbyId)
            ?? throw new NotFoundException("Lobby not found");

        if (lobby.State != LobbyState.Submitting)
        {
            throw new InvalidOperationException("Lobby is not in submission phase");
        }

        var participation = lobby.Participants.FirstOrDefault(p => p.UserId == userId && !p.IsKicked)
            ?? throw new InvalidOperationException("User is not a participant in this lobby");

        if (participation.Submissions != null && participation.Submissions.Count != 0)
        {
            throw new InvalidOperationException("User has already submitted a track");
        }

        if (dto.DurationSeconds <= 0 || dto.DurationSeconds > lobby.SubmissionTime.TotalSeconds / 2)
        {
            throw new ValidationException("Duration seconds must be a positive value and not exceed half the submission time limit");
        }

        var submission = new Submission
        {
            Value = dto.Value,
            ParticipationId = participation.Id,
            DurationSeconds = dto.DurationSeconds,
            LobbyId = dto.LobbyId
        };

        await context.Submissions.AddAsync(submission);

        // Initialize the collection if null and add the submission locally
        participation.Submissions ??= [];

        await context.SaveChangesAsync();
        // Check if all connected participants have a submission
        if (lobby.Participants.Where(p => p.IsConnected && !p.IsKicked).All(p => p.Submissions.Count != 0))
        {
            backgroundJobClient.Delete(lobby.JobId);
            await lobbyService.TransitionToVotingAsync(lobby.Id);
        }
    }

    public async Task UpdateValueAsync(Guid id, SubmissionUpdateDto dto, Guid userId)
    {
        var fluentValidationResult = await updateValidator.ValidateAsync(dto);
        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }

        var submission = await context.Submissions
                .Include(s => s.Participant)
                    .ThenInclude(p => p!.Lobby)
                .FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new NotFoundException("Submission not found");

        if (submission.Participant?.UserId != userId)
        {
            throw new UnauthorizedAccessException("User is not the owner of this submission");
        }

        if (submission.Participant?.Lobby?.State != LobbyState.Submitting)
        {
            throw new InvalidOperationException("Lobby is not in submission phase");
        }

        submission.Value = dto.Value;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var submission = await context.Submissions
            .Include(s => s.Lobby)
            .FirstOrDefaultAsync(s => s.Id == id)
                ?? throw new NotFoundException("Submission not found");

        if (submission.Lobby!.State != LobbyState.Submitting)
        {
            throw new InvalidOperationException("Lobby is not in submission phase");
        }

        context.Submissions.Remove(submission);
        await context.SaveChangesAsync();
    }
}