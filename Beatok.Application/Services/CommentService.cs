using AutoMapper;
using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Comment;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Application.Services;

public class CommentService(IApplicationDbContext context, IValidator<CreateCommentDto> validator,
    IMapper mapper): ICommentService
{
    public async Task CreateAsync(Guid authorId, Guid targetUserId, CreateCommentDto dto)
    {
        var fluentValidation = await validator.ValidateAsync(dto);
        if (!fluentValidation.IsValid)
            throw new ValidationException(fluentValidation.Errors);
        
        if (authorId == targetUserId)
            throw new BadRequestException("You cannot comment on yourself");
        
        if (!await context.Users.AnyAsync(u => u.Id == authorId))
            throw new NotFoundException("Author not found");
        if (!await context.Users.AnyAsync(u => u.Id == targetUserId))
            throw new NotFoundException("Taget user not found");

        await context.Comments.AddAsync(new Comment
        {
            AuthorId = authorId,
            TargetUserId = targetUserId,
            Content = dto.Content
        });
        await context.SaveChangesAsync();
    }

    public async Task<PageResult<CommentDto>> GetCommentsAsync(Guid targetUserId, int page, int pageSize)
    {
        if (!await context.Users.AnyAsync(u => u.Id == targetUserId))
            throw new NotFoundException("Target user not found");
        
        var comments = await context.Comments
            .Include(c => c.Author)
            .Where(c => c.TargetUserId == targetUserId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var totalCount = await context.Comments
            .CountAsync(c => c.TargetUserId == targetUserId);

        return new PageResult<CommentDto>
        {
            Items = mapper.Map<List<CommentDto>>(comments),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}