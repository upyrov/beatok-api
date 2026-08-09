namespace Beatok.Application.DTOs.Category;

public record CategoryDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int RandomSoundsCount { get; set; }
}