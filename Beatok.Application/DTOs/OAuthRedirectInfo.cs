namespace Beatok.Application.DTOs;

public record OAuthRedirectInfo
{
    public required string RedirectUrl { get; set; }
    public required string State { get; set; }
}