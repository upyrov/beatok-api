using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Repositories;

public interface ISubmissionRepository
{
    Task CreateAsync(Submission submission);
    Task<Submission?> GetByIdAsync(Guid submissionId);
    Task UpdateValueAsync(Guid submissionId, string value);
}