namespace Beatok.Application.DTOs.User;

public record UserSignupDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}