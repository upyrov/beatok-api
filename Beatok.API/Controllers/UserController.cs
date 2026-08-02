using System.Security.Claims;
using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Comment;
using Beatok.Application.DTOs.User;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController(IUserService userService, ICommentService commentService) : ControllerBase
    {
        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png"];
        
        [HttpGet("upload")]
        [Authorize]
        public ActionResult<PictureUploadDto> GetUploadUrl([FromQuery] string extension, [FromQuery] string contentType)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                return BadRequest("File extension is required");
            }

            if (string.IsNullOrWhiteSpace(contentType))
            {
                return BadRequest("Content type is required");
            }

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

            var dto = userService.GenerateUploadUrl(normalizedExtension, contentType);
            return Ok(dto);
        }

        
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<MeDto>> GetMe()
        {
            var userId =  User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            return Ok(await userService.GetMeAsync(Guid.Parse(userId)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById([FromRoute] Guid id)
        {
            return Ok(await userService.GetUserByIdAsync(id));
        }

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto dto)
        {
            var userId =  User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            await userService.UpdateAsync(Guid.Parse(userId), dto);
            return Ok();
        }
        
        [HttpPost("{id}/comments")]
        [Authorize]
        public async Task<IActionResult> AddComment([FromRoute] Guid id, [FromBody] CreateCommentDto dto)
        {
            var authorId =  User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            await commentService.CreateAsync(Guid.Parse(authorId), id, dto);
            return Ok();
        }

        [HttpGet("{id}/comments")]
        public async Task<ActionResult<PageResult<CommentDto>>> GetComments(
            [FromRoute] Guid id, [FromQuery] PaginationParams paginationParams)
        {
            var pageResult = await commentService
                .GetCommentsAsync(id, paginationParams.Page, paginationParams.PageSize);
            return Ok(pageResult);
        }
    }
}
