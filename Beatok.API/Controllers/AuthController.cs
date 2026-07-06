using Beatok.Application.DTOs.User;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(UserRegisterDto dto)
        {
            await authService.RegisterAsync(dto);
            return Ok();
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(UserLoginDto dto)
        {
            var authResult = await authService.LoginAsync(dto);
            
            SetCookie(authResult.AccessToken, authResult.RefreshToken, authResult.Expires);
            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var token = Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }
            
            var authResult = await authService.RefreshTokenAsync(token);
            SetCookie(authResult.AccessToken, authResult.RefreshToken, authResult.Expires);
            return Ok();
        }

        [HttpPost("sign-out")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return Ok();
        }
        
        private void SetCookie(string token, string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expires
            };

            Response.Cookies.Append("jwt", token, cookieOptions);
            Response.Cookies.Append("refresh_token", refreshToken, cookieOptions);
        }
    }
}
