using Beatok.Application.Interfaces.Repositories;
using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Infrastructure.Persistence.Repositories;

public class KitRepository(ApplicationDbContext context) : IKitRepository
{
    public async Task CreateAsync(Kit kit)
    {
        await context.Kits.AddAsync(kit);
    }

    public async Task<IEnumerable<Kit>> GetAllAsync()
    {
        return await context.Kits
            .Include(k => k.Categories)
            .ThenInclude(f => f.Sounds)
            .Include(k => k.Genres)
            .ToListAsync();
    }

    public async Task<Kit?> GetByIdAsync(Guid id)
    {
        return await context.Kits.FindAsync(id);
    }


    public async Task<Kit?> GetAsync()
    {
        return await context.Kits.Include(k => k.Categories)
            .ThenInclude(f => f.Sounds)
            .OrderBy(r => EF.Functions.Random())
            .FirstOrDefaultAsync();
    }

    public async Task UpdateNameAsync(Guid kitId, string name)
    {
        await context.Kits
            .Where(k => k.Id == kitId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(k => k.Name, name));
    }

    public void Delete(Kit kit)
    {
        context.Kits.Remove(kit);
    }
}