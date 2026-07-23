using AutoMapper;
using Beatok.Application.DTOs.User;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
    }
}