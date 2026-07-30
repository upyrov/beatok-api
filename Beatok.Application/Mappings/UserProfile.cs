using AutoMapper;
using Beatok.Application.DTOs.User;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Rating, opt
                => opt.MapFrom(src => src.Mu - src.Sigma * 3));
    }
}