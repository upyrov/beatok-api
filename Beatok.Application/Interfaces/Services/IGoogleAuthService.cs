using Beatok.Application.DTOs;

namespace Beatok.Application.Interfaces.Services;

public interface IGoogleAuthService
{
    string GenerateOAuthUrlRedirectUrl();
    Task<GoogleTokenResponse> ExchangeCodeForTokenAsync(string code);
    Task<GoogleUserInfo> GetUserInfoAsync(string accessToken);
}