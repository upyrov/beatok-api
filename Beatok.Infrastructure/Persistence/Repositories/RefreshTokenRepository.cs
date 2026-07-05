using Beatok.Application.Interfaces.Repositories;
using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(ApplicationDbContext context): IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken token)
    {
        await context.RefreshTokens.AddAsync(token);
    }

    public async Task<RefreshToken?> GetByHashAsync(string tokenHash)
    {
        return await context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);  
    }

    public async Task<int> DeleteExpiredAsync()
    {
        return await context.RefreshTokens
            .Where(t => t.Expires < DateTime.UtcNow)
            .ExecuteDeleteAsync();
    }

    public void Delete(RefreshToken token)
    {
        context.RefreshTokens.Remove(token);
    }
}