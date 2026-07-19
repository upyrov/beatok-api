using Beatok.Application.Interfaces.Repositories;
using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Infrastructure.Persistence.Repositories;

public class SubmissionRepository(ApplicationDbContext context) : ISubmissionRepository
{
    public async Task CreateAsync(Submission submission)
    {
        await context.Submissions.AddAsync(submission);
    }

    public async Task<Submission?> GetByIdAsync(Guid submissionId)
    {
        return await context.Submissions
            .Include(s => s.Participant)
                .ThenInclude(p => p!.Lobby)
            .Include(s => s.Scores)
            .FirstOrDefaultAsync(s => s.Id == submissionId);
    }

    public async Task UpdateValueAsync(Guid submissionId, string value)
    {
        await context.Submissions.Where(s => s.Id == submissionId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(submission => submission.Value, value));
    }
}