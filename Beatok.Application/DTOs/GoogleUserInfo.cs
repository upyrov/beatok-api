using System.Text.Json.Serialization;

namespace Beatok.Application.DTOs;

public record GoogleUserInfo
{
    [JsonPropertyName("email")]
    public required string Email { get; set; } 

    [JsonPropertyName("given_name")]
    public  required string Name { get; set; }
    
    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; set; }
}