using Beatok.API.Attributes;
using Beatok.Application.DTOs.Category;
using Beatok.Application.DTOs.Submission;
using Beatok.Application.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Beatok.API.Hubs
{
    public interface ILobbyClient
    {
        Task ParticipantJoined(UserDto user);
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