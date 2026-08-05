using Beatok.Application.DTOs.Submission;

namespace Beatok.Application.Interfaces.Services;

public interface ISubmissionService
{
    SubmissionUploadDto GenerateUploadUrl(string fileExtension, string contentType);
    Task CreateAsync(CreateSubmissionDto dto, string userId);
    Task UpdateValueAsync(Guid id, SubmissionUpdateDto dto, string userId);
    Task DeleteAsync(Guid id);
}