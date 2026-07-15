namespace Beatok.Application.DTOs.Kit;

public record KitDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}