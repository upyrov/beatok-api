using System.Security.Claims;
using Beatok.API.Attributes;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet("me")]
        [Authorize]
        [ImplicitAnonymous]
        public async Task<IActionResult> GetMe()
        {
            var userId =  User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            return Ok(await userService.GetUserByIdAsync(Guid.Parse(userId)));
        }
    }
}
