using Beatok.Application.Interfaces.Repositories;
using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Infrastructure.Persistence.Repositories;

public class SoundRepository(ApplicationDbContext context) : ISoundRepository
{
    public async Task CreateAsync(Sound sound)
    {
        await context.Sounds.AddAsync(sound);
    }

    public async Task<Sound?> GetByIdAsync(Guid id)
    {
        return await context.Sounds.FindAsync(id);
    }

    public async Task<IEnumerable<Sound>> GetAllByCategoryIdAsync(Guid id)
    {
        return await context.Sounds.Where(s => s.CategoryId == id).ToListAsync();
    }

    public async Task UpdateValueAsync(Guid soundId, string value)
    {
        await context.Sounds.Where(s => s.Id == soundId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(sound => sound.Value, value));
    }

    public void Delete(Sound sound)
    {

        context.Sounds.Remove(sound);
    }
}