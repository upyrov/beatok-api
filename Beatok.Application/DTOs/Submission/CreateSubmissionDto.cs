namespace Beatok.Application.DTOs.Submission;

public record CreateSubmissionDto
{
    public required string Value { get; set; }
    public int DurationSeconds { get; set; }
    public Guid LobbyId { get; set; }
}