using AutoMapper;
using Beatok.Application.DTOs.Submission;
using Beatok.Application.DTOs.User;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Application.Services;

public class UserService(IApplicationDbContext context, IMapper mapper, 
    IStorage storage): IUserService
{
    public PictureUploadDto GenerateUploadUrl(string fileExtension, string contentType)
    {
        if (!fileExtension.StartsWith('.'))
        {
            fileExtension = $".{fileExtension}";
        }

        var fileKey = $"pictures/{Guid.NewGuid()}{fileExtension}";
        var uploadUrl = storage.GeneratePresignedUploadUrl(fileKey, TimeSpan.FromMinutes(15), contentType);

        return new PictureUploadDto
        {
            UploadUrl = uploadUrl,
            FileKey = fileKey
        };
    }
    
    public async Task UpdateLastActiveAtAsync(Guid userId)
    {
        await context.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(s => 
                s.SetProperty(u => u.LastActiveAt, DateTime.UtcNow));
    }

    public async Task<UserDto> GetUserByIdAsync(Guid userId)
    {
        var user = await context.Users.FindAsync(userId)
            ?? throw new UserNotFoundException();
        return mapper.Map<UserDto>(user);
    }
}