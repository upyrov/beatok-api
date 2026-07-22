using Beatok.Application.Interfaces.Repositories;
using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Infrastructure.Persistence.Repositories;

public class CommentRepository(ApplicationDbContext context): ICommentRepository
{
    public async Task CreateAsync(Comment comment)
    {
        await context.Comments.AddAsync(comment);
    }

    public async Task<int> CountByUserId(Guid userId)
    {
        return await context.Comments
            .Where(c => c.TargetUserId == userId)
            .CountAsync();
    }

    public async Task<List<Comment>> GetByUserIdAsync(Guid userId, int page, int pageSize)
    {
        return await context.Comments
            .Include(c => c.Author)
            .Where(c => c.TargetUserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}