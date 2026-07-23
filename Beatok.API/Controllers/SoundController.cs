using Beatok.Application.DTOs.Sound;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("sounds")]
    [ApiController]
    [Authorize]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    public class SoundController(ISoundService soundService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSoundDto dto)
        {
            await soundService.CreateAsync(dto);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SoundDto>>> GetAllByCategoryId([FromQuery] Guid id)
        {
            var sounds = await soundService.GetAllByCategoryIdAsync(id);
            return Ok(sounds);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateValue([FromQuery] Guid id, [FromBody] UpdateSoundDto dto)
        {
            await soundService.UpdateValueAsync(id, dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await soundService.DeleteAsync(id);
            return Ok();
        }
    }
}
