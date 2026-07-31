namespace Beatok.Application.DTOs.User;

public record UserUpdateDto
{
    public required string Name { get; set; }
    public string? Picture { get; set; }
}