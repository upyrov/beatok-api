namespace Beatok.Domain.Entities;

public class Participation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool IsConnected { get; set; } = true;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public string? ConnectionId { get; set; }
    public Guid UserId { get; set; }
    public Guid LobbyId { get; set; }
    public User? User { get; set; }
    public Lobby? Lobby { get; set; }
    public ICollection<Submission> Submissions { get; set; } = [];
    public ICollection<Score> Scores { get; set; } = [];
}