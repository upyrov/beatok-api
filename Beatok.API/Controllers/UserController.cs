using System.Security.Claims;
using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Comment;
using Beatok.Application.DTOs.Lobby;
using Beatok.Application.DTOs.User;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController(IUserService userService, ILobbyService lobbyService,
        ICommentService commentService) : ControllerBase
    {
        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".jfif"];
        
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
            var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            return Ok(await userService.GetMeAsync(userId));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProfileDto>> GetById([FromRoute] string id, [FromQuery] int? year)
        {
            return Ok(await userService.GetByIdAsync(id, year));
        }

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto dto)
        {
            var userId =  User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            await userService.UpdateAsync(userId, dto);
            return Ok();
        }

        [HttpGet("{id:guid}/activity")]
        public async Task<ActionResult<IEnumerable<LobbyDto>>> GetHistory([FromRoute] string id, 
            [FromQuery] DateTime date)
        {
            var result = await lobbyService.GetByUserIdAsync(id, date);
            return Ok(result);
        }

        [HttpPost("{id:guid}/comments")]
        [Authorize]
        public async Task<IActionResult> AddComment([FromRoute] string id, [FromBody] CreateCommentDto dto)
        {
            var authorId =  User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            await commentService.CreateAsync(authorId, id, dto);
            return Ok();
        }

        [HttpGet("{id:guid}/comments")]
        public async Task<ActionResult<PageResult<CommentDto>>> GetComments(
            [FromRoute] string id, [FromQuery] PaginationParams paginationParams)
        {
            var pageResult = await commentService
                .GetCommentsAsync(id, paginationParams.Page, paginationParams.PageSize);
            return Ok(pageResult);
        }
    }
}
