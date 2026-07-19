namespace Beatok.Application.DTOs.Submission;

public record UpdateSubmissionDto
{
    public required string Value { get; set; }
    public int DurationSeconds { get; set; }
}