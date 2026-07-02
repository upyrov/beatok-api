using Beatok.Application.DTOs.User;

namespace Beatok.Application.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(UserRegisterDto dto);
}