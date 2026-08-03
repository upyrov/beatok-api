namespace Beatok.Application.DTOs;

public record ExternalUserInfo
{
    public required string Email { get; set; } 
    public  required string Name { get; set; }
    public bool EmailVerified { get; set; }
}