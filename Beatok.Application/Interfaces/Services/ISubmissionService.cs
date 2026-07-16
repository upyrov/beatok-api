using Beatok.Application.DTOs.Submission;

namespace Beatok.Application.Interfaces.Services;

public interface ISubmissionService
{
    Task CreateAsync(CreateSubmissionDto dto, Guid userId);
    Task UpdateValueAsync(Guid id, UpdateSubmissionDto dto);
}