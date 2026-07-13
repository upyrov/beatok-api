using Beatok.Application.DTOs.Kit;

namespace Beatok.Application.DTOs.Category;

public record CreateCategoryDto
{
    public required string Name { get; set; }
    public required KitDto Kit { get; set; }
}