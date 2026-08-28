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
                => opt.MapFrom<PresignedPictureUrlResolver<UserDto>>());

        CreateMap<UserUpdateDto, User>();

        CreateMap<User, MeDto>()
            .ForMember(dest => dest.Picture, opt
                => opt.MapFrom<PresignedPictureUrlResolver<MeDto>>());

        CreateMap<User, ProfileDto>()
            .IncludeBase<User, UserDto>()
            .ForMember(dest => dest.Activity, opt => opt.Ignore())
            .ForMember(dest => dest.AvailableYears, opt => opt.Ignore());
    }
}