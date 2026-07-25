using AutoMapper;
using Beatok.Application.DTOs.Category;
using Beatok.Domain.Entities;

namespace Beatok.Application.Mappings;

public class CategoryProfile: Profile
{
    public CategoryProfile()
    {
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<Category, CategoryDto>();
    }
}