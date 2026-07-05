using Beatok.Application.DTOs.Genre;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces.Repositories;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;

namespace Beatok.Application.Services;

public class GenreService(IGenreRepository genreRepository,
    IValidator<CreateGenreDto> validator): IGenreService
{
    public async Task CreateAsync(CreateGenreDto dto)
    {
        var fluentValidation = await validator.ValidateAsync(dto);

        if (!fluentValidation.IsValid)
        {
            throw new ValidationException(fluentValidation.Errors);
        }

        await genreRepository.CreateAsync(new Genre
        {
            Name = dto.Name
        });
    }

    public async Task<IEnumerable<GenreDto>> GetAllAsync()
    {
        var genres = await genreRepository.GetAllAsync();
        return genres.Select(g => new GenreDto
        {
            Id = g.Id,
            Name = g.Name
        });
    }

    public async Task DeleteAsync(int id)
    {
        var genre = await genreRepository.GetByIdAsync(id);
        if (genre == null)
        {
            throw new NotFoundException("Genre not found");
        }
        
        await genreRepository.DeleteAsync(genre);
    }
}