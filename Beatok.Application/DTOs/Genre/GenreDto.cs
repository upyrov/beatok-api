namespace Beatok.Application.DTOs.Genre;

public record GenreDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}