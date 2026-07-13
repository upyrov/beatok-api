using Beatok.API.Attributes;
using Beatok.Application.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Beatok.API.Hubs
{
    public interface ILobbyClient
    {
        // TODO: Replace sounds and submissions with corresponding DTOs
        void ParticipantJoined(UserDto user);
        void ParticipantLeft(UserDto user);
        void OwnerChanged(Guid ownerId);
        void MMRWithheld();
        void MessageReceived(string content, UserDto sender);
        void Started(List<string> sounds);
        void SubmissionRegistered(string userSubmission);
        void VotingStarted(List<string> submissions);
        void VoteRegistered(string userVote);
        void Ended(UserDto winner, string submission);
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