using AutoMapper;
using Beatok.Application.DTOs.Kit;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings;

public class KitProfile: Profile
{
    public KitProfile()
    {
        CreateMap<Kit, KitDto>();
    }
}