namespace Beatok.Application.DTOs.Sound;

public record SoundUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}