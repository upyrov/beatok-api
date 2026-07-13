namespace Beatok.Application.DTOs.Sound;

public record SoundDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Value { get; set; }
}