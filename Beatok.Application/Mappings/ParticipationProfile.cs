using AutoMapper;
using Beatok.Application.DTOs;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings;

public class ParticipationProfile : Profile
{
    public ParticipationProfile()
    {
        CreateMap<Participation, ParticipationDto>();
    }
}