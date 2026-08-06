using Beatok.Application.DTOs.Sound;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("sounds")]
    [ApiController]
    [Authorize]
    public class SoundController(ISoundService soundService) : ControllerBase
    {
        private static readonly string[] AllowedExtensions = [".mp3", ".wav", ".flac", ".ogg", ".m4a", ".aac"];

        [HttpGet("upload")]
        public ActionResult<SoundUploadDto> GetUploadUrl([FromQuery] string extension, [FromQuery] string contentType)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                return BadRequest("File extension is required");
            }

            if (string.IsNullOrWhiteSpace(contentType))
            {
                return BadRequest("Content type is required");
            }

            // Normalize the extension to ensure it starts with a dot and is lowercase
            var normalizedExtension = extension.StartsWith('.')
                ? extension.ToLowerInvariant()
                : $".{extension.ToLowerInvariant()}";

            if (!AllowedExtensions.Contains(normalizedExtension))
            {
                return BadRequest(new
                {
                    message = "Invalid file type",
                    allowedExtensions = AllowedExtensions
                });
            }

            var dto = soundService.GenerateUploadUrl(normalizedExtension, contentType);
            return Ok(dto);
        }

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

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> UpdateValue([FromRoute] Guid id, [FromBody] SoundUpdateDto dto)
        {
            await soundService.UpdateValueAsync(id, dto);
            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await soundService.DeleteAsync(id);
            return Ok();
        }
    }
}
