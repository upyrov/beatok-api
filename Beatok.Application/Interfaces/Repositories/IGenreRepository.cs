using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Repositories;

public interface IGenreRepository
{
    Task CreateAsync(Genre genre);
    Task<IEnumerable<Genre>> GetAllAsync();
    Task<Genre?> GetByIdAsync(int id);
    Task DeleteAsync(Genre genre);
}