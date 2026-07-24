using Beatok.Application.DTOs.Sound;
using Beatok.Application.DTOs.Submission;
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
        private static readonly string[] AllowedExtensions = [".mp3", ".wav", ".flac", ".ogg", ".m4a", ".aac"];

        [HttpGet("upload")]
        public ActionResult<SubmissionUploadDto> GetUploadUrl([FromQuery] string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                return BadRequest("File extension is required");
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

            var dto = soundService.GenerateUploadUrl(normalizedExtension);
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
