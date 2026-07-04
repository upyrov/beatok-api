using Beatok.Application.Interfaces.Repositories;
using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Infrastructure.Persistence.Repositories;

public class GenreRepository(ApplicationDbContext context): IGenreRepository
{
    public async Task CreateAsync(Genre genre)
    {
        await context.Genres.AddAsync(genre);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        return await context.Genres.ToListAsync();
    }

    public async Task<Genre?> GetByIdAsync(int id)
    {
        return await context.Genres.FindAsync(id);
    }

    public async Task DeleteAsync(Genre genre)
    {
        context.Genres.Remove(genre);
        await context.SaveChangesAsync();
    }
}