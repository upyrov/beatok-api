using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Comment;

namespace Beatok.Application.Interfaces.Services;

public interface ICommentService
{
    Task CreateAsync(string authorId, string targetUserId, CreateCommentDto dto);
    Task<PageResult<CommentDto>> GetCommentsAsync(string targetUserId, int page, int pageSize);
}