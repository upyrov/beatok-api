using System.Security.Claims;
using Beatok.API.Attributes;
using Beatok.Application.DTOs.Category;
using Beatok.Application.DTOs.Submission;
using Beatok.Application.DTOs.User;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Beatok.API.Hubs
{
    public interface ILobbyClient
    {
        Task ParticipantJoined(UserDto user);
        Task ParticipantRejoined(UserDto user);
        Task ParticipantLeft(UserDto user);
        Task OwnerChanged(Guid ownerId);
        Task MMRWithheld(); 
        Task MessageReceived(string content, UserDto sender);
        Task Started(ICollection<RandomCategoryDto> categories);
        Task SubmissionRegistered(SubmissionDto userSubmission);
        Task VotingStarted(ICollection<SubmissionDto> submissions);
        Task VoteRegistered(string userVote);
        Task Ended(UserDto winner, SubmissionDto submission);
    }

    [Authorize]
    [ImplicitAnonymous]
    public class LobbyHub(ILobbyService lobbyService) : Hub<ILobbyClient>
    {
        public async Task Join(string roomName)
        {
            var userId = Guid.Parse(
                Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            await lobbyService
                .SetConnectionIdAsync(Guid.Parse(roomName), userId, Context.ConnectionId);
            
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public Task Leave(string roomName)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await lobbyService.DisconnectAsync(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}