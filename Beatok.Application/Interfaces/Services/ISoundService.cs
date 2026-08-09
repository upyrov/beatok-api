using Beatok.Application.DTOs.Sound;

namespace Beatok.Application.Interfaces.Services;

public interface ISoundService
{
    SoundUploadDto GenerateUploadUrl(string fileExtension, string contentType);
    Task CreateAsync(CreateSoundDto dto);
    Task<IEnumerable<SoundDto>> GetAllByCategoryIdAsync(Guid categoryId);
    Task UpdateAsync(Guid id, SoundUpdateDto dto);
    Task DeleteAsync(Guid id);
}