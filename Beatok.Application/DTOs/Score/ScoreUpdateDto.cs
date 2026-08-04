namespace Beatok.Application.DTOs.Score;

public record ScoreUpdateDto
{
    public required short Value { get; set; }
}