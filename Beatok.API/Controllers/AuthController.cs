using System.Security.Claims;
using AutoMapper;
using Beatok.Application.DTOs;
using Beatok.Application.DTOs.User;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController(IAuthService authService, IGoogleAuthService googleAuthService, 
        IMapper mapper) : ControllerBase
    {
        [HttpGet("google/url")]
        public IActionResult GetGoogleAuthUrl()
        {
            return Ok(googleAuthService.GenerateOAuthUrlRedirectUrl());
        }

        [HttpGet("google/callback")]
        public async Task<IActionResult> GoogleAuthCallback([FromQuery] string code)
        {
            Guid? userIdClaim = null;
            if (Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id))
                userIdClaim = id;
            
            var token = await googleAuthService.ExchangeCodeForTokenAsync(code);
            var userInfo = await googleAuthService.GetUserInfoAsync(token.AccessToken);
            
            var AuthResultDto = await authService.
                AuthenticateExternalUserAsync(mapper.Map<ExternalUserInfo>(userInfo), userIdClaim);
            
            SetCookie(AuthResultDto.AccessToken, AuthResultDto.RefreshToken, AuthResultDto.Expires);
            return Redirect("https://localhost:5173/");
        }
        
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(UserSignupDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid? userId = null;
            if (Guid.TryParse(userIdClaim, out var id))
            {
                userId = id;
            }
            await authService.SignUpAsync(dto, userId);
            return Ok();
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(UserSigninDto dto)
        {
            var AuthResultDto = await authService.SignInAsync(dto);
            
            SetCookie(AuthResultDto.AccessToken, AuthResultDto.RefreshToken, AuthResultDto.Expires);
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
            
            var AuthResultDto = await authService.RefreshTokenAsync(token);
            SetCookie(AuthResultDto.AccessToken, AuthResultDto.RefreshToken, AuthResultDto.Expires);
            return Ok();
        }

        [HttpPost("sign-out")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            Response.Cookies.Delete("refresh_token");
            return Ok();
        }
        
        private void SetCookie(string token, string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = expires
            };

            Response.Cookies.Append("jwt", token, cookieOptions);
            Response.Cookies.Append("refresh_token", refreshToken, cookieOptions);
        }
    }
}
