namespace Beatok.Application.DTOs;

public class AuthResult
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
}