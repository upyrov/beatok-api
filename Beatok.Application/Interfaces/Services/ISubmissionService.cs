using Beatok.Application.DTOs.Submission;

namespace Beatok.Application.Interfaces.Services;

public interface ISubmissionService
{
    SubmissionUploadDto GenerateUploadUrl(string fileExtension, string contentType);
    Task CreateAsync(CreateSubmissionDto dto, Guid userId);
    Task UpdateValueAsync(Guid id, UpdateSubmissionDto dto, Guid userId);
    Task DeleteAsync(Guid id);
}