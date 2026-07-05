using Beatok.Application.DTOs.Genre;

namespace Beatok.Application.Interfaces.Services;

public interface IGenreService
{
    Task CreateAsync(CreateGenreDto dto);
    Task<IEnumerable<GenreDto>> GetAllAsync();
    Task DeleteAsync(int id);
}