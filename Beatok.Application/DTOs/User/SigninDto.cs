namespace Beatok.Application.DTOs.User;

public record SigninDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}