using Beatok.Application.DTOs.Genre;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("genres")]
    [ApiController]
    public class GenreController(IGenreService genreService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGenreDto dto)
        {
            await genreService.CreateAsync(dto);
            return Ok();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await genreService.GetAllAsync());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await genreService.DeleteAsync(id);
            return Ok();
        }
    }
}
