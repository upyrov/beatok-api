using Beatok.API.Attributes;
using Beatok.Application.DTOs.Genre;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("genres")]
    [ApiController]
    public class GenreController(IGenreService genreService) : ControllerBase
    {
        [HttpPost]
        [Admin]
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
        [Admin]
        public async Task<IActionResult> UpdateName([FromRoute] Guid id, [FromBody] GenreUpdateDto dto)
        {
            await genreService.UpdateNameAsync(id, dto);
            return Ok();
        }
        
        [HttpDelete("{id:guid}")]
        [Admin]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await genreService.DeleteAsync(id);
            return Ok();
        }
    }
}
