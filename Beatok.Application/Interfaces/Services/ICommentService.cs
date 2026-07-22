using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Comment;

namespace Beatok.Application.Interfaces.Services;

public interface ICommentService
{
    Task CreateAsync(Guid authorId, Guid targetUserId, CreateCommentDto dto);
    Task<PageResult<CommentDto>> GetCommentsAsync(Guid targetUserId, int page, int pageSize);
}