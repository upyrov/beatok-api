using Beatok.Application.DTOs.Category;
using Beatok.Application.DTOs.Kit;
using Beatok.Application.DTOs.Sound;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;

namespace Beatok.Application.Services;

public class KitService(IUnitOfWork unitOfWork, 
    IValidator<CreateKitDto> validator) : IKitService
{
    public async Task CreateAsync(CreateKitDto dto)
    {
        var fluentValidationResult = await validator.ValidateAsync(dto);

        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }

        await unitOfWork.Kits.CreateAsync(new Kit
        {
            Name = dto.Name
        });
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<KitDto>> GetAllAsync()
    {
        var kits = await unitOfWork.Kits.GetAllAsync();
        return kits.Select(k => new KitDto
        {
            Id = k.Id,
            Name = k.Name,
            Categories = [.. k.Categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Sounds = [.. c.Sounds.Select(s => new SoundDto
                {
                    Id = s.Id,
                    Value = s.Value
                })]
            })]
        });
    }

    public async Task<KitDto> GetAsync()
    {
        var kit = await unitOfWork.Kits.GetAsync();
        if (kit == null)
        {
            throw new NotFoundException("Kit not found");
        }

        return new KitDto
        {
            Id = kit.Id,
            Name = kit.Name,
             Categories = [.. kit.Categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Sounds = [.. c.Sounds.Select(s => new SoundDto
                {
                    Id = s.Id,
                    Value = s.Value
                })]
            })]
        };
    }


    public async Task DeleteAsync(Guid id)
    {
        var kit = await unitOfWork.Kits.GetByIdAsync(id) 
            ?? throw new NotFoundException("Kit not found");

        unitOfWork.Kits.Delete(kit);
        await unitOfWork.SaveChangesAsync();
    }
}
