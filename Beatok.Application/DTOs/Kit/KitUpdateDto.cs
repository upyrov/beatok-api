namespace Beatok.Application.DTOs.Kit;

public record KitUpdateDto
{
    public required string Name { get; set; }
    public IEnumerable<Guid> GenreIds { get; set; } = [];
}