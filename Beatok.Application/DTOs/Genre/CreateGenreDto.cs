namespace Beatok.Application.DTOs.Genre;

public record CreateGenreDto
{
    public required string Name { get; set; }
}