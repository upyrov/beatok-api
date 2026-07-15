using Beatok.Application.DTOs.User;

namespace Beatok.Application.Interfaces.Services;

public interface IUserService
{
    Task UpdateLastActiveAtAsync(Guid userId);
    Task<UserDto> GetUserByIdAsync(Guid userId);
}