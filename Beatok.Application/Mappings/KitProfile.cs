using AutoMapper;
using Beatok.Application.DTOs.Kit;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings;

public class KitProfile: Profile
{
    public KitProfile()
    {
        CreateMap<CreateKitDto, Kit>();
        
        CreateMap<Kit, KitDto>();
    }
}