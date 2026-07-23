using Beatok.Application.DTOs.Genre;

namespace Beatok.Application.DTOs.Kit;

public record KitDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required IEnumerable<GenreDto> Genres { get; set; }
}