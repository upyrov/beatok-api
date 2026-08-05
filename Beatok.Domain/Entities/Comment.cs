namespace Beatok.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string AuthorId { get; set; }
    public User? Author { get; set; }
    public required string TargetUserId { get; set; }
    public User? TargetUser { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}