using AutoMapper;
using Beatok.Application.DTOs.Submission;
using Beatok.Application.Mappings.Resolvers;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings;

public class SubmissionProfile: Profile
{
    public SubmissionProfile()
    {
        CreateMap<Submission, SubmissionDto>()
            .ForMember(dest => dest.Value, opt
                => opt.MapFrom<PresignedSubmissionUrlResolver<SubmissionDto>>());
    }
}