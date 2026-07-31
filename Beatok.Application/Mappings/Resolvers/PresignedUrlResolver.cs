using AutoMapper;
using Beatok.Application.DTOs.User;
using Beatok.Application.Interfaces;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings.Resolvers;

public class PresignedUrlResolver(IStorage storage): IValueResolver<User, UserDto, string?>
{
    public string? Resolve(User source, UserDto destination, string? destMember, ResolutionContext context)
    {
        return source.Picture is null 
            ? null
            : storage.GeneratePresignedUrl(source.Picture, TimeSpan.FromDays(1));
    }
}