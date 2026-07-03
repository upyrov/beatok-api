using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<bool> ExistsByEmailAsync(string email);
    Task<User?> GetByEmailAsync(string email);
    Task UpdateLastActiveAtAsync(Guid userId);
    Task<User?> GetByIdAsync(Guid id);
    Task<int> DeleteExpiredAnonymousUsersAsync(DateTime threshold);
}