using AutoMapper;
using Beatok.Application.DTOs.Lobby;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings;

public class LobbyProfile: Profile
{
    public LobbyProfile()
    {
        CreateMap<Lobby, LobbyWithParticipantsDto>()
            .ForMember(dest => dest.Participants, opt 
                => opt.MapFrom(src => src.Participants.Select(p => p.User)));

        CreateMap<Lobby, LobbyDto>()
            .ForMember(dest => dest.ParticipantCount, opt
                => opt.MapFrom(src => src.Participants.Count));
    }
}