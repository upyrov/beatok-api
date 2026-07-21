namespace Beatok.Application.DTOs.Comment;

public record CreateCommentDto
{
    public required string Content { get; set; }
}