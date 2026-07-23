using Beatok.API.Hubs;
using Beatok.Application.DTOs.Category;
using Beatok.Application.DTOs.Score;
using Beatok.Application.DTOs.User;
using Beatok.Application.DTOs.Submission;
using Beatok.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Beatok.API.Notifications;

public class SignalRLobbyNotifier(IHubContext<LobbyHub, ILobbyClient> hubContext)
    : ILobbyNotifier
{
    public async Task ParticipantJoinedAsync(Guid lobbyId, UserDto user)
    {
        await hubContext.Clients
            .Group(lobbyId.ToString())
            .ParticipantJoined(user);
    }
    
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

    public async Task ParticipantRejoinedAsync(Guid lobbyId, Guid userId)
    {
        await hubContext.Clients
            .Group(lobbyId.ToString())
            .ParticipantRejoined(userId);
    }

    public async Task ParticipantLeftAsync(Guid lobbyId, Guid userId)
    {
        await hubContext.Clients
            .Group(lobbyId.ToString())
            .ParticipantLeft(userId);   
    }

    public async Task OwnerChangedAsync(Guid lobbyId, Guid newOwnerId)
    {
        await hubContext.Clients
            .Group(lobbyId.ToString())
            .OwnerChanged(newOwnerId);
    }

    public async Task VoteRegisteredAsync(ScoreDto score)
    {
        await hubContext.Clients
            .Group(score.LobbyId.ToString())
            .VoteRegistered(score);
    }

    public async Task EndedAsync(Guid? winnerId, SubmissionDto? submission, Guid lobbyId)
    {
        await hubContext.Clients
            .Group(lobbyId.ToString())
            .Ended(winnerId, submission);
    }
}