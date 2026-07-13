namespace Beatok.Application.DTOs.Genre;

public record GenreDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
}