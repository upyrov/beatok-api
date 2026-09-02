using AutoMapper;
using Beatok.Application.DTOs;
using Beatok.Application.Interfaces;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings.Resolvers;

public class PresignedPictureUrlResolver<T>(IStorage storage): IValueResolver<User, T, PictureDto?>
{
    public PictureDto? Resolve(User source, T destination, PictureDto? destMember, ResolutionContext context)
    {
        return string.IsNullOrWhiteSpace(source.Picture)
            ? null
            : new PictureDto
            {
                Url = storage.GeneratePresignedUrl($"pictures/{source.Picture}", TimeSpan.FromDays(1)),
                Key = source.Picture
            };
    }
}