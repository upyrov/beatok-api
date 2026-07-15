using Beatok.Application.DTOs.Genre;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;

namespace Beatok.Application.Services;

public class GenreService(IUnitOfWork unitOfWork,
    IValidator<CreateGenreDto> validator): IGenreService
{
    public async Task CreateAsync(CreateGenreDto dto)
    {
        var fluentValidation = await validator.ValidateAsync(dto);

        if (!fluentValidation.IsValid)
        {
            throw new ValidationException(fluentValidation.Errors);
        }

        await unitOfWork.Genres.CreateAsync(new Genre
        {
            Name = dto.Name
        });
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<GenreDto>> GetAllAsync()
    {
        var genres = await unitOfWork.Genres.GetAllAsync();
        return genres.Select(g => new GenreDto
        {
            Id = g.Id,
            Name = g.Name
        });
    }

    public async Task DeleteAsync(Guid id)
    {
        var genre = await unitOfWork.Genres.GetByIdAsync(id);
        if (genre == null)
        {
            throw new NotFoundException("Genre not found");
        }
        
        unitOfWork.Genres.Delete(genre);
        await unitOfWork.SaveChangesAsync();
    }
}