using AutoMapper;
using Beatok.Application.Interfaces;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings.Resolvers;

public class PresignedUrlResolver<T>(IStorage storage): IValueResolver<User, T, string?>
{
    public string? Resolve(User source, T destination, string? destMember, ResolutionContext context)
    {
        return source.Picture is null 
            ? null
            : storage.GeneratePresignedUrl($"pictures/{source.Picture}", TimeSpan.FromDays(1));
    }
}