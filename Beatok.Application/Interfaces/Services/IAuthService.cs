using Beatok.Application.DTOs;
using Beatok.Application.DTOs.User;

namespace Beatok.Application.Interfaces.Services;

public interface IAuthService
{
    Task SignUpAsync(UserSignupDto dto, Guid? userId);
    Task<AuthResultDto> SignInAsync(UserSigninDto dto);
    Task<AuthResultDto> SignInAnonymousAsync();
    Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
    Task<AuthResultDto> AuthenticateExternalUserAsync(ExternalUserInfo userInfo, Guid? userIdClaim);
}