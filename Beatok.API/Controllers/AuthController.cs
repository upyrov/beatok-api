using System.Security.Claims;
using AutoMapper;
using Beatok.Application.DTOs;
using Beatok.Application.DTOs.User;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Beatok.API.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController(IAuthService authService, IGoogleAuthService googleAuthService, 
        IMapper mapper) : ControllerBase
    {
        private readonly CookieOptions baseCookieOptions = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
        };

        [HttpGet("google/url")]
        public ActionResult<string> GetGoogleAuthUrl()
        {
            var redirect = googleAuthService.GenerateOAuthUrlRedirectUrl();
            Response.Cookies.Append(
                "oauth_state",
                redirect.State,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(5)
                });
            return Ok(redirect.RedirectUrl);
        }

        [HttpGet("google/callback")]
        public async Task<IActionResult> GoogleAuthCallback([FromQuery] string code, [FromQuery] string state)
        {
            var expectedState = Request.Cookies["oauth_state"];
            if (string.IsNullOrEmpty(expectedState) || expectedState != state)
            {
                throw new BadRequestException("Invalid oauth state.");
            }
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
            };
            Response.Cookies.Delete("oauth_state", baseCookieOptions);
            
            Guid? userIdClaim = null;
            if (Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id))
                userIdClaim = id;
            
            var token = await googleAuthService.ExchangeCodeForTokenAsync(code);
            var userInfo = await googleAuthService.GetUserInfoAsync(token.AccessToken);
            
            var AuthResultDto = await authService.
                AuthenticateExternalUserAsync(mapper.Map<ExternalUserInfo>(userInfo), userIdClaim);
            
            SetCookie(AuthResultDto.AccessToken, AuthResultDto.RefreshToken, AuthResultDto.Expires);
            return Ok();
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
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
            };

            Response.Cookies.Delete("jwt", baseCookieOptions);
            Response.Cookies.Delete("refresh_token", baseCookieOptions);
            return Ok();
        }
        
        private void SetCookie(string token, string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = baseCookieOptions.HttpOnly,
                Secure = baseCookieOptions.Secure,
                SameSite = baseCookieOptions.SameSite,
                Expires = expires
            };

            Response.Cookies.Append("jwt", token, cookieOptions);
            Response.Cookies.Append("refresh_token", refreshToken, cookieOptions);
        }
    }
}
