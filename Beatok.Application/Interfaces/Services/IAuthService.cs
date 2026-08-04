using Beatok.Application.DTOs;
using Beatok.Application.DTOs.User;

namespace Beatok.Application.Interfaces.Services;

public interface IAuthService
{
    Task SignUpAsync(SignupDto dto, Guid? userId);
    Task<AuthResultDto> SignInAsync(SigninDto dto);
    Task<AuthResultDto> SignInAnonymousAsync();
    Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
    Task<AuthResultDto> AuthenticateExternalUserAsync(ExternalUserInfo userInfo, Guid? userIdClaim);
}