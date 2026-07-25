using AutoMapper;
using Beatok.Application.DTOs.Sound;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;

namespace Beatok.Application.Services;

public class SoundService(IUnitOfWork unitOfWork,
    IValidator<CreateSoundDto> createValidator, IValidator<UpdateSoundDto> updateValidator,
    IMapper mapper, IStorage storage)
    : ISoundService
{
    public SoundUploadDto GenerateUploadUrl(string fileExtension, string contentType)
    {
        // Standardize the extension format (e.g., "mp3" -> ".mp3")
        if (!fileExtension.StartsWith('.'))
        {
            fileExtension = $".{fileExtension}";
        }

        var fileKey = $"sounds/{Guid.NewGuid()}{fileExtension}";
        var uploadUrl = storage.GeneratePresignedUploadUrl(fileKey, TimeSpan.FromMinutes(15), contentType);

        return new SoundUploadDto
        {
            UploadUrl = uploadUrl,
            FileKey = fileKey
        };
    }

    public async Task CreateAsync(CreateSoundDto dto)
    {
        var fluentValidationResult = await createValidator.ValidateAsync(dto);

        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }

        var category = await unitOfWork.Categories.GetByIdAsync(dto.CategoryId) 
            ?? throw new BadRequestException("Category not found");

        await unitOfWork.Sounds.CreateAsync(mapper.Map<Sound>(dto));
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<SoundDto>> GetAllByCategoryIdAsync(Guid categoryId)
    {
        var sounds = await unitOfWork.Sounds.GetAllByCategoryIdAsync(categoryId);
        return mapper.Map<IEnumerable<SoundDto>>(sounds);
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