using AutoMapper;
using Beatok.Application.DTOs.Sound;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Application.Services;

public class SoundService(IApplicationDbContext context,
    IValidator<CreateSoundDto> createValidator, IValidator<SoundUpdateDto> updateValidator,
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

        var fileKey = $"{Guid.NewGuid()}{fileExtension}";
        var uploadUrl = storage.GeneratePresignedUploadUrl($"sounds/{fileKey}", TimeSpan.FromMinutes(15), contentType);

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

        var category = await context.Categories.FindAsync(dto.CategoryId) 
            ?? throw new BadRequestException("Category not found");

        await context.Sounds.AddAsync(mapper.Map<Sound>(dto));
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<SoundDto>> GetAllByCategoryIdAsync(Guid categoryId)
    {
        var sounds = await context.Sounds
            .Where(s => s.CategoryId == categoryId)
            .ToListAsync();
        foreach (var sound in sounds)
        {
            sound.Value = storage.GeneratePresignedUrl($"sounds/{sound.Value}", TimeSpan.FromHours(1));
        }
        return mapper.Map<IEnumerable<SoundDto>>(sounds);
    }

    public async Task UpdateAsync(Guid id, SoundUpdateDto dto)
    {
        var fluentValidationResult = await updateValidator.ValidateAsync(dto);
        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }

        var sound = await context.Sounds.FindAsync(id)
            ?? throw new NotFoundException("Sound not found");

        if (!string.IsNullOrWhiteSpace(dto.Value))
        {
            await storage.DeleteFileAsync($"sounds/{sound.Value}");
            sound.Value = dto.Value;
        }

        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            sound.Name = dto.Name;
        }
        
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var sound = await context.Sounds.FindAsync(id)
            ?? throw new NotFoundException("Sound not found");

        await storage.DeleteFileAsync($"sounds/{sound.Value}");
        context.Sounds.Remove(sound);
        await context.SaveChangesAsync();
    }
}