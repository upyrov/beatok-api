using Beatok.Application.DTOs.User;

namespace Beatok.Application.DTOs.Comment;

public record CommentDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required UserDto Author { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}