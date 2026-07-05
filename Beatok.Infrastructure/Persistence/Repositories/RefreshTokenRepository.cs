using Beatok.Application.Interfaces.Repositories;
using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(ApplicationDbContext context): IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken token)
    {
        await context.RefreshTokens.AddAsync(token);
        await context.SaveChangesAsync();   
    }

    public async Task<RefreshToken?> GetAsync(string tokenHash)
    {
        return await context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);  
    }
}