using Beatok.Application.DTOs.Category;
using Beatok.Application.DTOs.User;
using Beatok.Application.DTOs.Submission;

namespace Beatok.Application.Interfaces;

public interface ILobbyNotifier
{
    Task ParticipantJoinedAsync(Guid lobbyId, UserDto user);
    Task ParticipantRejoinedAsync(Guid lobbyId, UserDto user);
    Task ParticipantLeftAsync(Guid lobbyId, UserDto user);
    Task OwnerChangedAsync(Guid lobbyId, Guid newOwnerId);
    // Task MMRWithheldAsync();
    // Task MessageReceivedAsync(string content, GetUserDto sender);
    Task StartedAsync(Guid lobbyId, ICollection<RandomCategoryDto> categories);
    Task SubmissionRegisteredAsync(SubmissionDto userSubmission);
    Task VotingStartedAsync(Guid lobbyId, ICollection<SubmissionDto> submissions);
    // Task VoteRegisteredAsync(string userVote);
    // Task EndedAsync(GetUserDto winner, string submission);
}