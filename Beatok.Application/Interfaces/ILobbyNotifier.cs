using Beatok.Application.DTOs.Category;
using Beatok.Application.DTOs.User;

namespace Beatok.Application.Interfaces;

public interface ILobbyNotifier
{
    Task ParticipantJoinedAsync(Guid lobbyId, UserDto user);
    Task ParticipantRejoinedAsync(Guid lobbyId, UserDto user);
    // Task ParticipantLeftAsync(Guid lobbyId, GetUserDto user);
    // Task OwnerChangedAsync(Guid lobbyId, Guid ownerId);
    // Task MMRWithheldAsync();
    // Task MessageReceivedAsync(Guid lobbyId, string content, GetUserDto sender);
    Task StartedAsync(Guid lobbyId, ICollection<RandomCategoryDto> categories);
    // Task SubmissionRegisteredAsync(Guid lobbyId, string userSubmission);
    Task VotingStartedAsync(Guid lobbyId, ICollection<string> submissions);
    // Task VoteRegisteredAsync(Guid lobbyId, string userVote);
    // Task EndedAsync(Guid lobbyId, GetUserDto winner, string submission);
}