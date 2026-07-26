namespace Beatok.Application.DTOs.Submission;

public record SubmissionDto
{
    public Guid Id { get; set; }
    public required string Value { get; set; }
    public Guid UserId { get; set; }
    public Guid LobbyId { get; set; }
}