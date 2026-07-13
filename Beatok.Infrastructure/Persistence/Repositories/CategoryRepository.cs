using Beatok.Application.Interfaces.Repositories;
using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Infrastructure.Persistence.Repositories;

public class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
{
    public async Task CreateAsync(Category category)
    {
        await context.Categories.AddAsync(category);
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await context.Categories.FindAsync(id);
    }

    public async Task UpdateNameAsync(Guid categoryId, string name)
    {
        await context.Categories
            .Where(c => c.Id == categoryId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(c => c.Name, name));
    }

    public void Delete(Category category)
    {
        context.Categories.Remove(category);
    }
}