using Beatok.Application.DTOs.User;

namespace Beatok.Application.Interfaces.Services;

public interface IUserService
{
    PictureUploadDto GenerateUploadUrl(string fileExtension, string contentType);
    Task EnsureExistsAsync(string userId, string name, bool isAnonymous);
    Task<bool> IsAdminAsync(string userId);
    Task UpdateLastActiveAtAsync(string userId);
    Task<ProfileDto> GetByIdAsync(string userId, int? year = null);
    Task<MeDto> GetMeAsync(string userId);
    Task UpdateAsync(string userId, UserUpdateDto dto);
}