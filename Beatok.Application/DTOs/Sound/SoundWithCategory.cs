using Beatok.Application.DTOs.Category;

namespace Beatok.Application.DTOs.Sound;

public record SoundWithCategory
{
    public Guid Id { get; set; }
    public required string Value { get; set; }
    public required CategoryDto Category { get; set; }
}