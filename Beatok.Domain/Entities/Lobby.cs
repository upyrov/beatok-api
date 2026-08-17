namespace Beatok.Domain.Entities;

public enum LobbyState
{
    Waiting,
    Submitting,
    Voting,
    Ended
}

public class Lobby
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public short ParticipantLimit { get; set; }
    public string? JobId { get; set; }
    public TimeSpan SubmissionTime { get; set; }
    public LobbyState State { get; set; } = LobbyState.Waiting;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime SubmissionStartedAt { get; set; }
    public DateTime VotingStartedAt { get; set; }
    public DateTime EndedAt { get; set; }

    public Guid GenreId { get; set; }
    public Genre? Genre { get; set; }
    public required string OwnerId { get; set; }
    public User? Owner { get; set; }
    public ICollection<Participation> Participants { get; set; } = [];
    public ICollection<Sound> Sounds { get; set; } = [];
    public ICollection<Submission> Submissions { get; set; } = [];
    public Guid? WinningSubmissionId { get; set; }
}