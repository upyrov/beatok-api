using Beatok.Application.DTOs.User;

namespace Beatok.Application.Interfaces.Services;

public interface IUserService
{
    PictureUploadDto GenerateUploadUrl(string fileExtension, string contentType);
    Task UpdateLastActiveAtAsync(Guid userId);
    Task<UserDto> GetUserByIdAsync(Guid userId);
    Task UpdateAsync(Guid userId, UserUpdateDto dto);
}