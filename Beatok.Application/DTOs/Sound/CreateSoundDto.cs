namespace Beatok.Application.DTOs.Sound;

public record CreateSoundDto
{
    public required string Value { get; set; }
    public required Guid CategoryId { get; set; }
}