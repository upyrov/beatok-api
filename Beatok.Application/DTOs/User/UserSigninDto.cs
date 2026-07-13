namespace Beatok.Application.DTOs.User;

public record UserSigninDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}