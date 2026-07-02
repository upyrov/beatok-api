using Beatok.Application.DTOs.User;
using Beatok.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserRegisterDto dto)
        {
            await authService.RegisterAsync(dto);
            return Ok();
        }
    }
}
