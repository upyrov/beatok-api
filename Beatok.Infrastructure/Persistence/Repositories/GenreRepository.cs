using Beatok.Application.Interfaces.Repositories;
using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Infrastructure.Persistence.Repositories;

public class GenreRepository(ApplicationDbContext context): IGenreRepository
{
    public async Task CreateAsync(Genre genre)
    {
        await context.Genres.AddAsync(genre);
    }

    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        return await context.Genres.ToListAsync();
    }

    public async Task<Genre?> GetByIdAsync(Guid id)
    {
        return await context.Genres.FindAsync(id);
    }

    public async Task<List<Genre>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        return await context.Genres
            .Where(g => ids.Contains(g.Id))
            .ToListAsync();
    }

    public void Delete(Genre genre)
    {
        context.Genres.Remove(genre);
    }
}