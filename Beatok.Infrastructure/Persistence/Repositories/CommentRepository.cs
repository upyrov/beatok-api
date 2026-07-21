using Beatok.Application.Interfaces.Repositories;
using Beatok.Domain.Entities;

namespace Beatok.Infrastructure.Persistence.Repositories;

public class CommentRepository(ApplicationDbContext context): ICommentRepository
{
    public async Task CreateAsync(Comment comment)
    {
        await context.Comments.AddAsync(comment);
    }
}