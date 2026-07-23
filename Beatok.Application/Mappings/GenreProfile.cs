using AutoMapper;
using Beatok.Application.DTOs.Genre;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings;

public class GenreProfile: Profile
{
    public GenreProfile()
    {
        CreateMap<CreateGenreDto, Genre>();
        
        CreateMap<Genre, GenreDto>();
    }
}