using Beatok.API.Hubs;
using Beatok.Application.DTOs.Category;
using Beatok.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Beatok.API.Notifications;

public class SignalRLobbyNotifier(IHubContext<LobbyHub, ILobbyClient> hubContext)
    : ILobbyNotifier
{
    public void Started(Guid lobbyId, ICollection<RandomCategoryDto> categories)
    {
        hubContext.Clients
            .Group(lobbyId.ToString())
            .Started(categories);
    }

    public void VotingStarted(Guid lobbyId, ICollection<string> submissions)
    {
        hubContext.Clients
            .Group(lobbyId.ToString())
            .VotingStarted(submissions);
    }
}