namespace Beatok.Domain.Entities;

public enum LobbyPhase
{
    NotStarted,
    Submission,
    Voting,
    End
}

public class Lobby
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public short ParticipantLimit { get; set; }
    public LobbyPhase Phase { get; set; } = LobbyPhase.NotStarted;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan SubmissionTimeLimit { get; set; }
    public TimeSpan VotingTimeLimit { get; set; }
    public Guid OwnerId { get; set; }
    public User? Owner { get; set; }
    public ICollection<Participation> Participants { get; set; } = [];
}