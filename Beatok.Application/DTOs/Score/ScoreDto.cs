namespace Beatok.Application.DTOs.Score;

public record ScoreDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public short Value { get; set; }
    public Guid LobbyId { get; set; }
    public Guid UserId { get; set; }
    public Guid SubmissionId { get; set; }
}