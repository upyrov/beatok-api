using Beatok.API.Attributes;
using Beatok.Application.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Beatok.API.Hubs
{
    public interface ILobbyClient
    {
        // TODO: Replace sounds and submissions with corresponding DTOs
        Task ParticipantJoined(GetUserDto user);
        Task ParticipantLeft(GetUserDto user);
        Task OwnerChanged(Guid ownerId);
        Task MMRWithheld(); 
        Task MessageReceived(string content, GetUserDto sender);
        Task Started(List<string> sounds);
        Task SubmissionRegistered(string userSubmission);
        Task VotingStarted(List<string> submissions);
        Task VoteRegistered(string userVote);
        Task Ended(GetUserDto winner, string submission);
    }

    [Authorize]
    [ImplicitAnonymous]
    public class LobbyHub : Hub<ILobbyClient>
    {
        public async Task Join(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public Task Leave(string roomName)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        //public override Task OnDisconnectedAsync(Exception? exception)
        //{
            // TODO: Set IsConnected to false for all user participations
        //}
    }
}