using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Repositories;

public interface IGenreRepository
{
    Task CreateAsync(Genre genre);
    Task<IEnumerable<Genre>> GetAllAsync();
    Task<Genre?> GetByIdAsync(Guid id);
    Task<List<Genre>> GetByIdsAsync(IEnumerable<Guid> ids);
    void Delete(Genre genre);
}