using Beatok.Application.DTOs.User;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;

namespace Beatok.Application.Services;

public class UserService(IUnitOfWork unitOfWork): IUserService
{
    public async Task UpdateLastActiveAtAsync(Guid userId)
    {
        await unitOfWork.Users.UpdateLastActiveAtAsync(userId);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<GetUserDto> GetUserByIdAsync(string userId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(Guid.Parse(userId));

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