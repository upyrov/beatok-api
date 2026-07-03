using System.Security.Claims;
using Beatok.API.Attributes;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        [HttpGet("me")]
        [ImplicitAnonymous]
        public async Task<IActionResult> GetMe()
        {
            var userId =  User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            return Ok(await userService.GetUserByIdAsync(userId));
        }
    }
}
