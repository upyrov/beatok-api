using AutoMapper;
using Beatok.Application.Interfaces;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings.Resolvers;

public class PresignedSoundUrlResolver<T>(IStorage storage): IValueResolver<Sound, T, string>
{
    public string Resolve(Sound source, T destination, string? destMember, ResolutionContext context)
    {
        return storage.GeneratePresignedUrl($"sounds/{source.Value}", TimeSpan.FromHours(1));
    }
}