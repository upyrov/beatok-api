namespace Beatok.Domain.Entities;

public class Score
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public short Value { get; set; }
    public Guid LobbyId { get; set; }
    public Lobby? Lobby { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Guid SubmissionId { get; set; }
    public Submission? Submission { get; set; }
}