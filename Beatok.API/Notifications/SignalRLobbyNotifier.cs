using Beatok.API.Hubs;
using Beatok.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Beatok.API.Notifications;

public class SignalRLobbyNotifier(IHubContext<LobbyHub, ILobbyClient> hubContext)
    : ILobbyNotifier
{
    public void Started(Guid lobbyId, List<string> sounds)
    {
        hubContext.Clients
            .Group(lobbyId.ToString())
            .Started(sounds);
    }

    public void VotingStarted(Guid lobbyId, List<string> submissions)
    {
        hubContext.Clients
            .Group(lobbyId.ToString())
            .VotingStarted(submissions);
    }
}