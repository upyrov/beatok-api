using AutoMapper;
using Beatok.Application.DTOs.Category;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Application.Services;

public class CategoryService(IApplicationDbContext context,
    IValidator<CreateCategoryDto> createValidator, IValidator<CategoryUpdateDto> updateValidator, 
    IMapper mapper)
    : ICategoryService
{
    public async Task CreateAsync(CreateCategoryDto dto)
    {
        var fluentValidationResult = await createValidator.ValidateAsync(dto);

        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }

        var kit = await context.Kits
            .Include(k => k.Genres)
            .FirstOrDefaultAsync(k => k.Id == dto.KitId);
        if (kit == null)
        {
            throw new NotFoundException("Kit not found");      
        }

        await context.Categories.AddAsync(mapper.Map<Category>(dto));
        await context.SaveChangesAsync();
    }

    public async Task <IEnumerable<CategoryDto>> GetAllByKitIdAsync(Guid id)
    {
        var categories = await context.Categories
            .Where(c => c.KitId == id).ToListAsync();
        return mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task UpdateNameAsync(Guid id, CategoryUpdateDto dto)
    {
        var fluentValidationResult = await updateValidator.ValidateAsync(dto);
        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }

        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new NotFoundException("Category not found");

        await context.Categories
            .Where(c => c.Id == category.Id)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(c => c.Name, dto.Name));
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new NotFoundException("Category not found");

        context.Categories.Remove(category);
        await context.SaveChangesAsync();
    }
}