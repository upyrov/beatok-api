using Beatok.Application.DTOs.Category;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;

namespace Beatok.Application.Services;

public class CategoryService(IUnitOfWork unitOfWork,
    IValidator<CreateCategoryDto> createValidator, IValidator<UpdateCategoryDto> updateValidator)
    : ICategoryService
{
    public async Task CreateAsync(CreateCategoryDto dto)
    {
        var fluentValidationResult = await createValidator.ValidateAsync(dto);

        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }

        await unitOfWork.Categories.CreateAsync(new Category
        {
            Name = dto.Name,
            KitId = dto.Kit.Id
        });
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateNameAsync(Guid id, UpdateCategoryDto dto)
    {
        var fluentValidationResult = await updateValidator.ValidateAsync(dto);
        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }

        var category = await unitOfWork.Categories.GetByIdAsync(id)
            ?? throw new NotFoundException("Category not found");

        await unitOfWork.Categories.UpdateNameAsync(category.Id, dto.Name);
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(id)
            ?? throw new NotFoundException("Category not found");

        unitOfWork.Categories.Delete(category);
        await unitOfWork.SaveChangesAsync();
    }
}