namespace Beatok.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AuthorId { get; set; }
    public User? Author { get; set; }
    public Guid TargetUserId { get; set; }
    public User? TargetUser { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
}