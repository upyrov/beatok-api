using Beatok.Application.DTOs.Category;

namespace Beatok.Application.Interfaces.Services;

public interface ICategoryService
{
    Task CreateAsync(CreateCategoryDto dto);
    Task<IEnumerable<CategoryDto>> GetAllByKitIdAsync(Guid id);
    Task UpdateAsync(Guid id, CategoryUpdateDto dto);
    Task DeleteAsync(Guid id);
}