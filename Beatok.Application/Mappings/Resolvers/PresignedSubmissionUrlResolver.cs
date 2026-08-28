using AutoMapper;
using Beatok.Application.Interfaces;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings.Resolvers;

public class PresignedSubmissionUrlResolver<T>(IStorage storage): IValueResolver<Submission, T, string>
{
    public string Resolve(Submission source, T destination, string? destMember, ResolutionContext context)
    {
        return storage.GeneratePresignedUrl($"submissions/{source.Value}", TimeSpan.FromHours(1));
    }
}