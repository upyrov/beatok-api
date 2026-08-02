using Beatok.Application.DTOs.Genre;
using Beatok.Application.DTOs.Sound;
using Beatok.Application.Interfaces.Services;
using Beatok.Application.Services;
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

        [HttpGet]
        public async Task<ActionResult<List<GenreDto>>> GetAll()
        {
            return Ok(await genreService.GetAllAsync());
        }

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> UpdateName([FromRoute] Guid id, [FromBody] UpdateGenreDto dto)
        {
            await genreService.UpdateNameAsync(id, dto);
            return Ok();
        }

        [Authorize(Roles = nameof(UserRole.Administrator))]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await genreService.DeleteAsync(id);
            return Ok();
        }
    }
}
