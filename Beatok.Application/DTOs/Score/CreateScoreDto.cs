namespace Beatok.Application.DTOs.Score;

public record CreateScoreDto
{
    public short Value { get; set; }
    public Guid SubmissionId { get; set; }
}