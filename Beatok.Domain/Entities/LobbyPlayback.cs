namespace Beatok.Domain.Entities;

public class LobbyPlaybackItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LobbyId { get; set; }
    public Lobby? Lobby { get; set; }
    public Guid SubmissionId { get; set; }
    public Submission? Submission { get; set; }
    public int Order { get; set; }
}