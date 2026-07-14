namespace Beatok.Application.DTOs.Kit;

public record UpdateKitDto
{
    public required string Name { get; set; }
    public IEnumerable<Guid> GenreIds { get; set; } = [];
}