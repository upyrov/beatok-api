using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task CreateAsync(Category category);
    Task<Category?> GetByIdAsync(Guid categoryId);
    Task UpdateNameAsync(Guid categoryId, string name);
    void Delete(Category category);
}