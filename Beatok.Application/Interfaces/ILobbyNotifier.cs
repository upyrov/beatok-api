using Beatok.Application.DTOs.Category;
using Beatok.Application.DTOs.Submission;
using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Sound;
using Beatok.Application.DTOs.User;

namespace Beatok.Application.Interfaces;

public interface ILobbyNotifier
{
    Task ParticipantJoinedAsync(Guid lobbyId, ParticipationDto participant);
    Task ParticipantConnectedAsync(Guid lobbyId, Guid userId);
    Task ParticipantLeftAsync(Guid lobbyId, Guid userId);
    Task ParticipantDisconnectedAsync(Guid lobbyId, Guid userId);
    Task OwnerChangedAsync(Guid lobbyId, Guid newOwnerId);
    Task MessageReceivedAsync(Guid lobbyId, Guid userId, string content);
    Task StartedAsync(Guid lobbyId, ICollection<SoundWithCategory> sounds);
    Task VotingStartedAsync(Guid lobbyId, TimeSpan votingTime, ICollection<SubmissionDto> submissions);
    Task EndedAsync(Guid lobbyId, Guid? winningSubmissionId, IEnumerable<RatingChangeDto> ratingChanges);
}