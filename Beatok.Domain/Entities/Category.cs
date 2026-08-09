namespace Beatok.Domain.Entities;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public int RandomSoundsCount { get; set; }
    public Guid KitId { get; set; }
    public Kit? Kit { get; set; }
    public ICollection<Sound> Sounds { get; set; } = [];
}