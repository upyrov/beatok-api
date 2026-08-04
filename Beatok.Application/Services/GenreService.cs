using AutoMapper;
using Beatok.Application.DTOs.Genre;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Application.Services;

public class GenreService(IApplicationDbContext context,
    IValidator<CreateGenreDto> validator, IMapper mapper): IGenreService
{
    public async Task CreateAsync(CreateGenreDto dto)
    {
        var fluentValidation = await validator.ValidateAsync(dto);

        if (!fluentValidation.IsValid)
        {
            throw new ValidationException(fluentValidation.Errors);
        }

        await context.Genres.AddAsync(mapper.Map<Genre>(dto));
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<GenreDto>> GetAllAsync()
    {
        var genres = await context.Genres.ToListAsync();
        return mapper.Map<IEnumerable<GenreDto>>(genres);
    }

    public async Task UpdateNameAsync(Guid id, GenreUpdateDto dto)
    {
        var genre = await context.Genres.FindAsync(id) ?? throw new NotFoundException("Genre not found");
        genre.Name = dto.Name;
        context.Genres.Update(genre);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var genre = await context.Genres.FindAsync(id) ?? throw new NotFoundException("Genre not found");
        context.Genres.Remove(genre);
        await context.SaveChangesAsync();
    }
}