using Beatok.Application.DTOs.User;

namespace Beatok.Application.Interfaces.Services;

public interface IUserService
{
    PictureUploadDto GenerateUploadUrl(string fileExtension, string contentType);
    Task CreateAsync(string userId, string name, bool isAnonymous, string email);
    Task<bool> ExistsAsync(string userId);
    Task UpdateLastActiveAtAsync(string userId);
    Task<ProfileDto> GetByIdAsync(string userId, int? year = null);
    Task<MeDto> GetMeAsync(string userId);
    Task UpdateAsync(string userId, UserUpdateDto dto);
}