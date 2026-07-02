using Beatok.Application.Interfaces.Repositories;
using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context): IUserRepository
{
    public async Task AddAsync(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
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
}