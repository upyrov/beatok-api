namespace Beatok.Application.DTOs.User;

public record UserRatingChangeDto
{
    public Guid UserId;
    public double RatingChange { get; set; }
};