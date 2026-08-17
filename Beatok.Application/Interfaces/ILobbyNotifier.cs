using Beatok.Application.DTOs.Submission;
using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Sound;
using Beatok.Application.DTOs.User;

namespace Beatok.Application.Interfaces;

public interface ILobbyNotifier
{
    Task ParticipantJoinedAsync(Guid lobbyId, ParticipationDto participant);
    Task ParticipantConnectedAsync(Guid lobbyId, string userId);
    Task ParticipantLeftAsync(Guid lobbyId, string userId);
    Task ParticipantDisconnectedAsync(Guid lobbyId, string userId);
    Task KickedReceivedAsync(string connectionId);
    Task OwnerChangedAsync(Guid lobbyId, string newOwnerId);
    Task MessageReceivedAsync(Guid lobbyId, string userId, string content);
    Task StartedAsync(Guid lobbyId, ICollection<SoundWithCategory> sounds);
    Task VotingStartedAsync(Guid lobbyId);
    Task SubmissionForPlaybackAsync(Guid lobbyId, SubmissionDto submission, DateTime startedAt);
    Task EndedAsync(Guid lobbyId, Guid? winningSubmissionId, IEnumerable<RatingChangeDto> ratingChanges);
}