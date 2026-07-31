namespace Beatok.Application.DTOs.User;

public record UserUpdateDto
{
    public string? Name { get; set; }
    public string? Picture { get; set; }
}