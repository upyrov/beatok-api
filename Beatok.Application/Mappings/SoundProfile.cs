using AutoMapper;
using Beatok.Application.DTOs.Sound;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings;

public class SoundProfile: Profile
{
    public SoundProfile()
    {
        CreateMap<CreateSoundDto, Sound>();
        CreateMap<Sound, SoundDto>();
    }
}