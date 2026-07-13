using Beatok.Application.DTOs.Sound;

namespace Beatok.Application.Interfaces.Services;

public interface ISoundService
{
    Task CreateAsync(CreateSoundDto dto);
    Task UpdateNameAsync(Guid id, UpdateSoundDto dto);
    Task DeleteAsync(Guid id);
}