using Beatok.Application.DTOs.Category;
using Beatok.Application.DTOs.User;

namespace Beatok.Application.Interfaces;

public interface ILobbyNotifier
{
    Task ParticipantJoinedAsync(Guid lobbyId, UserDto user);
    Task ParticipantRejoinedAsync(Guid lobbyId, UserDto user);
    Task ParticipantLeftAsync(Guid lobbyId, UserDto user);
    Task OwnerChangedAsync(Guid lobbyId, Guid newOwnerId);
    // Task MMRWithheldAsync();
    // Task MessageReceivedAsync(Guid lobbyId, string content, UserDto sender);
    Task StartedAsync(Guid lobbyId, ICollection<RandomCategoryDto> categories);
    // Task SubmissionRegisteredAsync(Guid lobbyId, string userSubmission);
    Task VotingStartedAsync(Guid lobbyId, ICollection<string> submissions);
    // Task VoteRegisteredAsync(Guid lobbyId, string userVote);
    // Task EndedAsync(Guid lobbyId, UserDto winner, string submission);
}