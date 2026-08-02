namespace Beatok.Application.DTOs.Score;

public record UpdateScoreDto
{
    public required short Value { get; set; }
}