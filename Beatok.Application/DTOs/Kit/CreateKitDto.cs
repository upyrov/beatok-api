using Beatok.Application.DTOs.Genre;

namespace Beatok.Application.DTOs.Kit;

public record CreateKitDto
{
    public required string Name { get; set; }
    public ICollection<GenreDto> Genres { get; set; } = [];
}