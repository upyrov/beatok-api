using Beatok.Application.DTOs;
using Beatok.Application.DTOs.User;

namespace Beatok.Application.Interfaces.Services;

public interface IAuthService
{
    Task RegisterAsync(UserRegisterDto dto);
    Task<AuthResult> LoginAsync(UserLoginDto dto);
    Task<AuthResult> LoginAnonymousAsync();
}