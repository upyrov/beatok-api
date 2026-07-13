namespace Beatok.Domain.Entities;

public class Sound
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Value { get; set; }
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
}
