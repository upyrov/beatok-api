using Beatok.API.Hubs;
using Beatok.Application.DTOs.Submission;
using Beatok.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Sound;
using Beatok.Application.DTOs.User;

namespace Beatok.API.Notifications;

public class SignalRLobbyNotifier(IHubContext<LobbyHub, ILobbyClient> hubContext)
    : ILobbyNotifier
{
    public async Task ParticipantJoinedAsync(Guid lobbyId, ParticipationDto participant)
    {
        await hubContext.Clients
            .Group(lobbyId.ToString())
            .ParticipantJoined(participant);
    }
    
    public async Task StartedAsync(Guid lobbyId, ICollection<SoundWithCategory> sounds)
    {
        await hubContext.Clients
            .Group(lobbyId.ToString())
            .Started(sounds);
    }

    public async Task VotingStartedAsync(Guid lobbyId, TimeSpan votingTime, ICollection<SubmissionDto> submissions)
    {
        await hubContext.Clients
            .Group(lobbyId.ToString())
            .VotingStarted(votingTime, submissions);
    }

    public async Task ParticipantConnectedAsync(Guid lobbyId, Guid userId)
    {
        await hubContext.Clients
            .Group(lobbyId.ToString())
            .ParticipantConnected(userId);
    }

    public async Task ParticipantLeftAsync(Guid lobbyId, Guid userId)
    {
        await hubContext.Clients
            .Group(lobbyId.ToString())
            .ParticipantLeft(userId);   
    }

    public async Task ParticipantDisconnectedAsync(Guid lobbyId, Guid userId)
    {
        await hubContext.Clients
            .Group(lobbyId.ToString())
            .ParticipantDisconnected(userId);
    }

    public async Task OwnerChangedAsync(Guid lobbyId, Guid newOwnerId)
    {
        await hubContext.Clients
            .Group(lobbyId.ToString())
            .OwnerChanged(newOwnerId);
    }

    public async Task EndedAsync(Guid lobbyId, Guid? winningSubmissionId, IEnumerable<RatingChangeDto> ratingChanges)
    {
        await hubContext.Clients
            .Group(lobbyId.ToString())
            .Ended(winningSubmissionId, ratingChanges);
    }

    public async Task MessageReceivedAsync(Guid lobbyId, Guid userId, string content)
    {
        await hubContext.Clients
            .Group(lobbyId.ToString())
            .MessageReceived(userId, content);
    }
}