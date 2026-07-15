using Beatok.Application.DTOs.Sound;

namespace Beatok.Application.DTOs.Category;

public record RandomCategoryDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required ICollection<SoundDto> Sounds { get; set; }
}