using Beatok.Application.Interfaces.Repositories;
using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context): IUserRepository
{
    public async Task AddAsync(User user)
    {
        await context.Users.AddAsync(user);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task UpdateLastActiveAtAsync(Guid userId)
    {
        await context.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(s => 
                s.SetProperty(u => u.LastActiveAt, DateTime.UtcNow));
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<int> DeleteExpiredAnonymousUsersAsync(DateTime threshold)
    {
        return await context.Users
            .Where(u => u.IsAnonymous
                        && u.LastActiveAt != null
                        && u.LastActiveAt < threshold)
            .ExecuteDeleteAsync();
    }
}