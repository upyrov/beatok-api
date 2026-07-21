using Beatok.Application.DTOs.Genre;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("genres")]
    [ApiController]
    public class GenreController(IGenreService genreService) : ControllerBase
    {
        [Authorize(Roles = nameof(UserRole.Administrator))]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGenreDto dto)
        {
            await genreService.CreateAsync(dto);
            return Ok();
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<GenreDto>>> GetAll()
        {
            return Ok(await genreService.GetAllAsync());
        }

        [Authorize(Roles = nameof(UserRole.Administrator))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await genreService.DeleteAsync(id);
            return Ok();
        }
    }
}
