using Beatok.Application.DTOs.Category;

namespace Beatok.Application.Interfaces;

public interface ILobbyNotifier
{
    // Task ParticipantJoinedAsync(GetUserDto user);
    // Task ParticipantLeftAsync(GetUserDto user);
    // Task OwnerChangedAsync(Guid ownerId);
    // Task MMRWithheldAsync();
    // Task MessageReceivedAsync(string content, GetUserDto sender);
    void Started(Guid lobbyId, ICollection<RandomCategoryDto> categories);
    // Task SubmissionRegisteredAsync(string userSubmission);
    void VotingStarted(Guid lobbyId, ICollection<string> submissions);
    // Task VoteRegisteredAsync(string userVote);
    // Task EndedAsync(GetUserDto winner, string submission);
}