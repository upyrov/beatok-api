using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Lobby;
using Beatok.Application.DTOs.Sound;
using Beatok.Application.DTOs.User;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Beatok.Application.DTOs.Submission;

namespace Beatok.API.Hubs
{
    public interface ILobbyClient
    {
        Task ParticipantJoined(ParticipationDto participant);
        Task ParticipantConnected(string userId);
        Task ParticipantLeft(string userId);
        Task KickedReceived();
        Task ParticipantDisconnected(string userId);
        Task OwnerChanged(string ownerId);
        Task MessageReceived(string senderId, string content);
        Task Started(ICollection<SoundWithCategory> sounds);
        Task VotingStarted();
        Task SubmissionForPlayback(SubmissionDto submission, long startedAt);
        Task Ended(Guid? winningSubmissionId, IEnumerable<RatingChangeDto> ratingChanges);
    }

    [Authorize]
    public class LobbyHub(ILobbyService lobbyService) : Hub<ILobbyClient>
    {
        public async Task<DetailedLobbyDto> Join(string lobbyId)
        {
            var userId = Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var lobby = await lobbyService.JoinAsync(Guid.Parse(lobbyId), userId, Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
            return lobby;
        }

        public async Task Leave(string lobbyId)
        {
            var userId = Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            await lobbyService.LeaveAsync(Guid.Parse(lobbyId), userId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);
        }

        public async Task SendMessage(Guid lobbyId, string content)
        {
            var userId = Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            await lobbyService.SendMessageAsync(lobbyId, userId, content);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await lobbyService.DisconnectAsync(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}