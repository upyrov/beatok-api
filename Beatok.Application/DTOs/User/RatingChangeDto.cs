namespace Beatok.Application.DTOs.User;

public record RatingChangeDto
{
    public required string UserId { get; set; }
    public double RatingChange { get; set; }
};