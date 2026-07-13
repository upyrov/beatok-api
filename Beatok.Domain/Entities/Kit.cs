namespace Beatok.Domain.Entities;

public class Kit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public ICollection<Genre> Genres { get; set; } = [];
    public ICollection<Category> Categories { get; set; } = [];
}