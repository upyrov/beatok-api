using AutoMapper;
using Beatok.Application.DTOs.Kit;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;

namespace Beatok.Application.Services;

public class KitService(IUnitOfWork unitOfWork, 
    IValidator<CreateKitDto> createValidator, IValidator<UpdateKitDto> updateValidator,
    IMapper mapper)
    : IKitService
{
    public async Task CreateAsync(CreateKitDto dto)
    {
        var fluentValidationResult = await createValidator.ValidateAsync(dto);

        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }

        var genres = await unitOfWork.Genres.GetByIdsAsync(dto.GenreIds);

        if (genres.Count != dto.GenreIds.Count())
        {
            throw new NotFoundException("One or more genres not found");
        }
    
        await unitOfWork.Kits.CreateAsync(mapper.Map<Kit>(dto));
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<KitDto>> GetAllAsync()
    {
        var kits = await unitOfWork.Kits.GetAllAsync();
        return mapper.Map<IEnumerable<KitDto>>(kits);
    }

    public async Task<Kit> GetRandomAsync()
    {
        var kit = await unitOfWork.Kits.GetRandomAsync();
        return kit ?? throw new NotFoundException("Kit not found");
    }

    public async Task UpdateAsync(Guid id, UpdateKitDto dto)
    {
        var fluentValidationResult = await updateValidator.ValidateAsync(dto);
        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }

        var kit = await unitOfWork.Kits.GetByIdAsync(id)
            ?? throw new NotFoundException("Kit not found");
        
        var genres = await unitOfWork.Genres.GetByIdsAsync(dto.GenreIds);
        if (genres.Count != dto.GenreIds.Count())
        {
            throw new NotFoundException("One or more genres not found");
        }
        kit.Name = dto.Name;
        kit.Genres = genres;
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var kit = await unitOfWork.Kits.GetByIdAsync(id) 
            ?? throw new NotFoundException("Kit not found");

        unitOfWork.Kits.Delete(kit);
        await unitOfWork.SaveChangesAsync();
    }
}
