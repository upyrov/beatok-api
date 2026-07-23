using AutoMapper;
using Beatok.Application.DTOs.Score;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings;

public class ScoreProfile: Profile
{
    public ScoreProfile()
    {
        CreateMap<Score, ScoreDto>();
    }
}