using AutoMapper;
using Beatok.Application.DTOs;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings;

public class LobbyPlaybackItemProfile: Profile
{
    public LobbyPlaybackItemProfile()
    {
        CreateMap<LobbyPlaybackItem, LobbyPlaybackItemDto>();
    }
}