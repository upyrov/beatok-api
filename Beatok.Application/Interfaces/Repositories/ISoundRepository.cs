using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Repositories;

public interface ISoundRepository
{
    Task CreateAsync(Sound sound);
    Task<Sound?> GetByIdAsync(Guid soundId);
    Task<IEnumerable<Sound>> GetAllByCategoryIdAsync(Guid id);
    Task UpdateValueAsync(Guid soundId, string value);
    void Delete(Sound sound);
}