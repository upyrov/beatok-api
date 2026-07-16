using Beatok.Application.DTOs.Sound;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;

namespace Beatok.Application.Services;

public class SoundService(IUnitOfWork unitOfWork,
    IValidator<CreateSoundDto> createValidator, IValidator<UpdateSoundDto> updateValidator)
    : ISoundService
{
    public async Task CreateAsync(CreateSoundDto dto)
    {
        var fluentValidationResult = await createValidator.ValidateAsync(dto);

        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }

        var category = await unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
        if (category == null)
            throw new BadRequestException("Category not found");

        await unitOfWork.Sounds.CreateAsync(new Sound
        {
            Value = dto.Value,
            CategoryId = dto.CategoryId
        });
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateValueAsync(Guid id, UpdateSoundDto dto)
    {
        var fluentValidationResult = await updateValidator.ValidateAsync(dto);
        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }

        var sound = await unitOfWork.Sounds.GetByIdAsync(id)
            ?? throw new NotFoundException("Sound not found");

        await unitOfWork.Sounds.UpdateValueAsync(sound.Id, dto.Value); 
    }

    public async Task DeleteAsync(Guid id)
    {
        var sound = await unitOfWork.Sounds.GetByIdAsync(id) 
            ?? throw new NotFoundException("Sound not found");

        unitOfWork.Sounds.Delete(sound);
        await unitOfWork.SaveChangesAsync();
    }
}