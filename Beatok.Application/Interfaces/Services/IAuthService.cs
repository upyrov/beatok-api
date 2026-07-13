using Beatok.Application.DTOs;
using Beatok.Application.DTOs.User;

namespace Beatok.Application.Interfaces.Services;

public interface IAuthService
{
    Task SignUpAsync(UserSignupDto dto);
    Task<AuthResultDto> SignInAsync(UserSigninDto dto);
    Task<AuthResultDto> SignInAnonymousAsync();
    Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
}