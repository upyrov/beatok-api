using AutoMapper;
using Beatok.Application.DTOs.User;
using Beatok.Application.Mappings.Resolvers;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Picture, opt
                => opt.MapFrom<PresignedUrlResolver>());
        CreateMap<UserUpdateDto, User>();
        CreateMap<User, MeDto>();
    }
}