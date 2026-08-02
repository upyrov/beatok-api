using System.Security.Claims;
using Beatok.Application.DTOs.Submission;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("submissions")]
    [ApiController]
    [Authorize]
    public class SubmissionController(ISubmissionService submissionService) : ControllerBase
    {
        private static readonly string[] AllowedExtensions = [".mp3", ".wav", ".flac", ".ogg", ".m4a", ".aac"];

        [HttpGet("upload")]
        public ActionResult<SubmissionUploadDto> GetUploadUrl([FromQuery] string extension, [FromQuery] string contentType)
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

            var dto = submissionService.GenerateUploadUrl(normalizedExtension, contentType);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSubmissionDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await submissionService.CreateAsync(dto, Guid.Parse(userId!));
            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateValue([FromRoute] Guid id, [FromBody] UpdateSubmissionDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await submissionService.UpdateValueAsync(id, dto, Guid.Parse(userId!));
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await submissionService.DeleteAsync(id);
            return Ok();
        }
    }
}