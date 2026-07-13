namespace Beatok.Domain.Entities;

public class Genre
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Kit> Kits { get; set; } = [];
}