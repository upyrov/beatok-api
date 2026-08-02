using Beatok.Application.DTOs.Kit;
using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Services;

public interface IKitService
{
    Task CreateAsync(CreateKitDto dto);
    Task<IEnumerable<KitDto>> GetAllAsync();
    Task<Kit> GetRandomAsync(Guid genreId);
    Task UpdateAsync(Guid id, UpdateKitDto dto);
    Task DeleteAsync(Guid id);
}