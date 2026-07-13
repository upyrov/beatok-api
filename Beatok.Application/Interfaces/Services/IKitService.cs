using Beatok.Application.DTOs.Kit;

namespace Beatok.Application.Interfaces.Services;

public interface IKitService
{
    Task CreateAsync(CreateKitDto dto);
    Task<IEnumerable<KitDto>> GetAllAsync();
    Task<KitDto> GetAsync();
    Task UpdateNameAsync(Guid id, UpdateKitDto dto);
    Task DeleteAsync(Guid id);
}