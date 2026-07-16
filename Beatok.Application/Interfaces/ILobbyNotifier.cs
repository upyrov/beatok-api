using Beatok.Application.DTOs.Category;
using Beatok.Application.DTOs.Submission;

namespace Beatok.Application.Interfaces;

public interface ILobbyNotifier
{
    // Task ParticipantJoinedAsync(GetUserDto user);
    // Task ParticipantLeftAsync(GetUserDto user);
    // Task OwnerChangedAsync(Guid ownerId);
    // Task MMRWithheldAsync();
    // Task MessageReceivedAsync(string content, GetUserDto sender);
    Task StartedAsync(Guid lobbyId, ICollection<RandomCategoryDto> categories);
    Task SubmissionRegisteredAsync(SubmissionDto userSubmission);
    Task VotingStartedAsync(Guid lobbyId, ICollection<SubmissionDto> submissions);
    // Task VoteRegisteredAsync(string userVote);
    // Task EndedAsync(GetUserDto winner, string submission);
}