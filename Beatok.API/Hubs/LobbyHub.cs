using System.Security.Claims;
using Beatok.API.Attributes;
using Beatok.Application.DTOs.Category;
using Beatok.Application.DTOs.Lobby;
using Beatok.Application.DTOs.Score;
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
        Task ParticipantRejoined(Guid userId);
        Task ParticipantLeft(Guid userId);
        Task OwnerChanged(Guid ownerId);
        Task MMRWithheld(); 
        Task MessageReceived(string content, Guid senderId);
        Task Started(ICollection<RandomCategoryDto> categories);
        Task SubmissionRegistered(SubmissionDto userSubmission);
        Task VotingStarted(ICollection<SubmissionDto> submissions);
        Task VoteRegistered(ScoreDto score);
        Task Ended(SubmissionDto? submission);
    }

    [Authorize]
    [ImplicitAnonymous]
    public class LobbyHub(ILobbyService lobbyService) : Hub<ILobbyClient>
    {
        public async Task<LobbyWithParticipantsDto> Join(string lobbyId)
        {
            var userId = Guid.Parse(
                Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var lobby = await lobbyService
                .SetConnectionIdAsync(Guid.Parse(lobbyId), userId, Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
            return lobby;
        }

        public Task Leave(string lobbyId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await lobbyService.DisconnectAsync(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string content, Guid lobbyId)
        {
            var userId = Guid.Parse(
                Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await lobbyService.SendMessageAsync(lobbyId, userId, content);
        }
    }
}