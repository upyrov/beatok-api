namespace Beatok.Application.DTOs.Submission;

public record SubmissionUploadDto
{
    public required string UploadUrl { get; set; }
    public required string FileKey { get; set; }
}