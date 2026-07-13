using Beatok.Application.DTOs.Category;

namespace Beatok.Application.Interfaces.Services;

public interface ICategoryService
{
    Task CreateAsync(CreateCategoryDto dto);
    Task UpdateNameAsync(Guid id, UpdateCategoryDto dto);
    Task DeleteAsync(Guid id);
}