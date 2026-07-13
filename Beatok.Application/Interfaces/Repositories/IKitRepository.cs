using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Repositories;

public interface IKitRepository
{
    Task CreateAsync(Kit kit);
    Task<IEnumerable<Kit>> GetAllAsync();
    Task<Kit?> GetByIdAsync(Guid kitId);
    Task<Kit?> GetRandomAsync();
    Task UpdateNameAsync(Guid kitId, string name);
    void Delete(Kit kit);
}