namespace Beatok.Application.DTOs;

public class JwtGenerateResult
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
}