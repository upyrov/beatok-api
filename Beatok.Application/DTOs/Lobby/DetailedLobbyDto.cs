using Beatok.Application.DTOs.Genre;
using Beatok.Application.DTOs.Sound;
using Beatok.Application.DTOs.Submission;
using Beatok.Domain.Entities;

namespace Beatok.Application.DTOs.Lobby;

public record DetailedLobbyDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public int ParticipantLimit { get; set; }
    public TimeSpan SubmissionTime { get; set; }
    public TimeSpan? VotingTime { get; set; }
    public LobbyState State { get; set; } = LobbyState.Waiting;

    public DateTime CreatedAt { get; set; }
    public DateTime SubmissionStartedAt { get; set; }
    public DateTime VotingStartedAt { get; set; }
    public DateTime EndedAt { get; set; }

    public required GenreDto Genre { get; set; }
    public required string OwnerId { get; set; }
    public IEnumerable<ParticipationDto> Participants { get; set; } = [];
    public IEnumerable<SoundWithCategory> Sounds { get; set; } = [];
    public IEnumerable<SubmissionDto> Submissions { get; set; } = [];
    public Guid? WinningSubmissionId { get; set; }
    public LobbyPlaybackItemDto? CurrentPlaybackItem { get; set; }
}