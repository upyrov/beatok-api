using Beatok.Application.DTOs.User;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces.Repositories;
using Beatok.Application.Interfaces.Services;

namespace Beatok.Application.Services;

public class UserService(IUserRepository userRepository): IUserService
{
    public async Task UpdateLastActiveAtAsync(Guid userId)
    {
        await userRepository.UpdateLastActiveAtAsync(userId);
    }

    public async Task<GetUserDto> GetUserByIdAsync(string userId)
    {
        var user = await userRepository.GetByIdAsync(Guid.Parse(userId));

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        return new GetUserDto
        {
            Name = user.Name
        };
    }
}