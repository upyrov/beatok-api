using Beatok.API.Hubs;
using Beatok.Application.DTOs.Category;
using Beatok.Application.DTOs.Submission;
using Beatok.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Beatok.API.Notifications;

public class SignalRLobbyNotifier(IHubContext<LobbyHub, ILobbyClient> hubContext)
    : ILobbyNotifier
{
    public async Task StartedAsync(Guid lobbyId, ICollection<RandomCategoryDto> categories)
    {
        await hubContext.Clients
            .Group(lobbyId.ToString())
            .Started(categories);
    }

    public async Task SubmissionRegisteredAsync(SubmissionDto userSubmission)
    {
        await hubContext.Clients
            .Group(userSubmission.LobbyId.ToString())
            .SubmissionRegistered(userSubmission);
    }

    public async Task VotingStartedAsync(Guid lobbyId, ICollection<SubmissionDto> submissions)
    {
        await hubContext.Clients
            .Group(lobbyId.ToString())
            .VotingStarted(submissions);
    }
}