namespace Beatok.Application.DTOs.User;

public record RatingChangeDto
{
    public Guid UserId;
    public double RatingChange { get; set; }
};