using System.Security.Claims;
using Beatok.API.Attributes;
using Beatok.Application.DTOs.Comment;
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

        [HttpPost("{id}/comments")]
        [Authorize]
        [ImplicitAnonymous]
        public async Task<IActionResult> AddComment([FromRoute] Guid id, [FromBody] CreateCommentDto dto)
        {
            var authorId =  User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            await commentService.CreateAsync(Guid.Parse(authorId), id, dto);
            return Ok();
        }
    }
}
