namespace Beatok.Application.DTOs.User;

public record RatingChangeDto
{
    public required string UserId;
    public double RatingChange { get; set; }
};