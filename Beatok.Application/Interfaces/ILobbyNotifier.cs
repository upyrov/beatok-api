using Beatok.Application.DTOs.Category;
using Beatok.Application.DTOs.Score;
using Beatok.Application.DTOs.User;
using Beatok.Application.DTOs.Submission;

namespace Beatok.Application.Interfaces;

public interface ILobbyNotifier
{
    Task ParticipantJoinedAsync(Guid lobbyId, UserDto user);
    Task ParticipantRejoinedAsync(Guid lobbyId, Guid userId);
    Task ParticipantLeftAsync(Guid lobbyId, Guid userId);
    Task OwnerChangedAsync(Guid lobbyId, Guid newOwnerId);
    // Task MMRWithheldAsync();
    Task MessageReceivedAsync(string content, Guid userId, Guid lobbyId);
    Task StartedAsync(Guid lobbyId, ICollection<RandomCategoryDto> categories);
    Task VotingStartedAsync(Guid lobbyId, ICollection<SubmissionDto> submissions);
    Task EndedAsync(Guid lobbyId, SubmissionDto? submission);
}