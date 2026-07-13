namespace Beatok.Domain.Entities;

public class GenreKit
{
    public Guid GenreId { get; set; }
    public Guid KitId { get; set; }
    public required Genre Genre { get; set; }
    public required Kit Kit { get; set; }   
}