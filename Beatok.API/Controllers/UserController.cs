using System.Security.Claims;
using Beatok.API.Attributes;
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
        [HttpGet("me")]
        [Authorize]
        [ImplicitAnonymous]
        public async Task<IActionResult> GetMe()
        {
            var userId =  User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            return Ok(await userService.GetUserByIdAsync(Guid.Parse(userId)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById([FromRoute] Guid id)
        {
            return Ok(await userService.GetUserByIdAsync(id));
        }
        
        [HttpPost("{id}/comments")]
        [Authorize]
        [ImplicitAnonymous]
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
