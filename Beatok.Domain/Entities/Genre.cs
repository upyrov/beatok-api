namespace Beatok.Domain.Entities;

public class Genre
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public ICollection<Kit> Kits { get; set; } = [];
}