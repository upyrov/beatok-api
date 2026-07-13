namespace Beatok.Application.DTOs;

public record AuthResultDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime Expires { get; set; }
}