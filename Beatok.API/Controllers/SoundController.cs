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
    public class SoundController(ISoundService soundService) : ControllerBase
    {
        [Authorize(Roles = nameof(UserRole.Administrator))]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSoundDto dto)
        {
            await soundService.CreateAsync(dto);
            return Ok();
        }

        [Authorize(Roles = nameof(UserRole.Administrator))]
        [HttpPut]
        public async Task<IActionResult> UpdateValue([FromQuery] Guid id, [FromBody] UpdateSoundDto dto)
        {
            await soundService.UpdateValueAsync(id, dto);
            return Ok();
        }

        [Authorize(Roles = nameof(UserRole.Administrator))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await soundService.DeleteAsync(id);
            return Ok();
        }
    }
}
