using AutoMapper;
using Beatok.Application.DTOs.Submission;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings;

public class SubmissionProfile: Profile
{
    public SubmissionProfile()
    {
        CreateMap<Submission, SubmissionDto>();
    }
}