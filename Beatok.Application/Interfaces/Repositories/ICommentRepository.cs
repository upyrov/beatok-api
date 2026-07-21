using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Repositories;

public interface ICommentRepository
{
    Task CreateAsync(Comment comment);
    Task<int> CountByUserId(Guid userId);
    Task<List<Comment>> GetByUserIdAsync(Guid userId, int page, int pageSize);
}