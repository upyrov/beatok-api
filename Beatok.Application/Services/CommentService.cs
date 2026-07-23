using AutoMapper;
using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Comment;
using Beatok.Application.DTOs.User;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;

namespace Beatok.Application.Services;

public class CommentService(IUnitOfWork unitOfWork, IValidator<CreateCommentDto> validator,
    IMapper mapper): ICommentService
{
    public async Task CreateAsync(Guid authorId, Guid targetUserId, CreateCommentDto dto)
    {
        var fluentValidation = await validator.ValidateAsync(dto);
        if (!fluentValidation.IsValid)
            throw new ValidationException(fluentValidation.Errors);
        
        if (authorId == targetUserId)
            throw new BadRequestException("You cannot comment on yourself");
        
        var author = await unitOfWork.Users.GetByIdAsync(authorId);
        if (author == null)
            throw new NotFoundException("Author not found");
        var targetUser = await unitOfWork.Users.GetByIdAsync(targetUserId);
        if (targetUser == null)
            throw new NotFoundException("Taget user not found");

        await unitOfWork.Comments.CreateAsync(new Comment
        {
            AuthorId = authorId,
            TargetUserId = targetUserId,
            Content = dto.Content
        });
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<PageResult<CommentDto>> GetCommentsAsync(Guid targetUserId, int page, int pageSize)
    {
        var user = await unitOfWork.Users.GetByIdAsync(targetUserId);
        if (user == null)
            throw new NotFoundException("Target user not found");
        
        var comments = await unitOfWork.Comments.GetByUserIdAsync(targetUserId, page, pageSize);
        var totalCount = await unitOfWork.Comments.CountByUserId(targetUserId);

        return new PageResult<CommentDto>
        {
            Items = mapper.Map<List<CommentDto>>(comments),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}