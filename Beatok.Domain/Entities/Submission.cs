namespace Beatok.Domain.Entities;

public class Submission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Value { get; set; }
    public int DurationSeconds { get; set; }
    public Guid ParticipationId { get; set; } 
    public Participation? Participant { get; set; }
    public Guid LobbyId { get; set; }
    public Lobby? Lobby { get; set; }
    public ICollection<Score> Scores { get; set; } = [];
}