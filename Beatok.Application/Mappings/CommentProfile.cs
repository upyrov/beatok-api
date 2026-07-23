using AutoMapper;
using Beatok.Application.DTOs.Comment;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings;

public class CommentProfile: Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, CommentDto>();
    }
}