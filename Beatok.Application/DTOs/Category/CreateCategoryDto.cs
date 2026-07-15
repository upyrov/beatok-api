namespace Beatok.Application.DTOs.Category;

public record CreateCategoryDto
{
    public required string Name { get; set; }
    public required Guid KitId { get; set; }
}