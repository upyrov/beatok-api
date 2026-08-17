namespace Beatok.Application.DTOs;

public record LobbyPlaybackItemDto
{
    public Guid SubmissionId { get; set; }
    public DateTime StartedAt { get; set; }
    public int Order { get; set; }
}