namespace Beatok.Application.DTOs.Category;

public record CategoryUpdateDto
{
    public required string Name { get; set; }
    public int RandomSoundsCount { get; set; }
}