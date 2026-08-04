namespace Beatok.Application.DTOs.User;

public record UserDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public double Rating { get; set; }
    public string? Picture { get; set; }
}