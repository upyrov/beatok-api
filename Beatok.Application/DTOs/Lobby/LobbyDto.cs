using Beatok.Domain.Entities;

namespace Beatok.Application.DTOs.Lobby;

public class LobbyDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public int GenreId { get; set; }
    public short ParticipantLimit { get; set; }
    public LobbyPhase Phase { get; set; } = LobbyPhase.NotStarted;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan SubmissionTimeLimit { get; set; }
    public TimeSpan VotingTimeLimit { get; set; }
    public Guid OwnerId { get; set; }
}