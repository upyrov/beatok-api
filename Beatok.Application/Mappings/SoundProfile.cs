using AutoMapper;
using Beatok.Application.DTOs.Sound;
using Beatok.Application.Mappings.Resolvers;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings;

public class SoundProfile: Profile
{
    public SoundProfile()
    {
        CreateMap<CreateSoundDto, Sound>();
        CreateMap<Sound, SoundDto>()
            .ForMember(dest => dest.Value, opt
                => opt.MapFrom<PresignedSoundUrlResolver<SoundDto>>());
        CreateMap<Sound, SoundWithCategory>()
            .ForMember(dest => dest.Value, opt
                => opt.MapFrom<PresignedSoundUrlResolver<SoundWithCategory>>());
    }
}