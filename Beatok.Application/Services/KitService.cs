using AutoMapper;
using Beatok.Application.DTOs.Kit;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Application.Services;

public class KitService(IApplicationDbContext context, 
    IValidator<CreateKitDto> createValidator, IValidator<KitUpdateDto> updateValidator,
    IMapper mapper, IStorage storage)
    : IKitService
{
    public async Task CreateAsync(CreateKitDto dto)
    {
        var fluentValidationResult = await createValidator.ValidateAsync(dto);

        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }

        var genreIds = dto.GenreIds.Distinct().ToList();
        var genres = await context.Genres
            .Where(g => genreIds.Contains(g.Id))
            .ToListAsync();
        
        if (genres.Count != genreIds.Count)
        {
            throw new NotFoundException("One or more genres not found");
        }

        var kit = new Kit
        {
            Name = dto.Name,
            Genres = genres
        };
    
        await context.Kits.AddAsync(kit);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<KitDto>> GetAllAsync()
    {
        var kits = await context.Kits
            .Include(k => k.Genres)
            .ToListAsync();
        return mapper.Map<IEnumerable<KitDto>>(kits);
    }

    public async Task<List<Guid>> GetRandomSoundIdsAsync(Guid genreId)
    {
        var kit = await context.Kits
            .AsNoTracking()
            .Where(k => k.Genres.Any(g => g.Id == genreId))
            .Include(k => k.Categories)
            .ThenInclude(c => c.Sounds)
            .OrderBy(_ => EF.Functions.Random())
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException("Kit not found");
    
        return kit.Categories
            .SelectMany(c => c.Sounds
                .OrderBy(_ => Guid.NewGuid())
                .Take(c.RandomSoundsCount))
            .Select(s => s.Id)
            .Distinct()
            .ToList();
    }

    public async Task UpdateAsync(Guid id, KitUpdateDto dto)
    {
        var fluentValidationResult = await updateValidator.ValidateAsync(dto);
        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }

        var kit = await context.Kits
                      .Include(k => k.Genres)
                      .FirstOrDefaultAsync(k => k.Id == id)
            ?? throw new NotFoundException("Kit not found");
        
        var genres = await context.Genres
            .Where(g => dto.GenreIds.Contains(g.Id))
            .ToListAsync();
        
        if (genres.Count != dto.GenreIds.Count())
        {
            throw new NotFoundException("One or more genres not found");
        }
        kit.Name = dto.Name;
        kit.Genres = genres;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var kit = await context.Kits
                      .Include(k => k.Categories)
                            .ThenInclude(c => c.Sounds)
                      .FirstOrDefaultAsync(k => k.Id == id)
            ?? throw new NotFoundException("Kit not found");

        foreach (var sound in kit.Categories.SelectMany(c => c.Sounds))
        {
            await storage.DeleteFileAsync($"sounds/{sound.Value}");
        }
        
        context.Kits.Remove(kit);
        await context.SaveChangesAsync();
    }
}
