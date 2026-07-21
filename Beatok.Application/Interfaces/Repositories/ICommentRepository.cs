using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Repositories;

public interface ICommentRepository
{
    Task CreateAsync(Comment comment);
}