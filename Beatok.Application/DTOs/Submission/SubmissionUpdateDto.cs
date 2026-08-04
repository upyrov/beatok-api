namespace Beatok.Application.DTOs.Submission;

public record SubmissionUpdateDto
{
    public required string Value { get; set; }
    public int DurationSeconds { get; set; }
}