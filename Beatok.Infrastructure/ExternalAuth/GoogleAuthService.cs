using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Authentication;
using Beatok.Application.DTOs;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Beatok.Infrastructure.ExternalAuth;

public class GoogleAuthService(
    IOptions<GoogleAuthOptions> options,
    IHttpClientFactory httpClientFactory) : IGoogleAuthService
{
    GoogleAuthOptions _options = options.Value;
    private const string AuthUrl = "https://accounts.google.com/o/oauth2/v2/auth";

    public OAuthRedirectInfo GenerateOAuthUrlRedirectUrl()
    {
        var state = Guid.NewGuid().ToString("N");
        
        var query = new Dictionary<string, string?>
        {
            ["redirect_uri"] = _options.RedirectUri,
            ["client_id"] = _options.ClientId,
            ["response_type"] = "code",
            ["scope"] = "openid email profile",
            ["access_type"] = "offline",
            ["prompt"] = "consent",
            ["state"] = state
        };
        var url = QueryHelpers.AddQueryString(AuthUrl, query);
        return new OAuthRedirectInfo
        {
            RedirectUrl = url,
            State = state
        };
    }

    public async Task<GoogleTokenResponse> ExchangeCodeForTokenAsync(string code)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
            ["code"] = code,
            ["redirect_uri"] = _options.RedirectUri,
            ["grant_type"] = "authorization_code"
        });

        var client = httpClientFactory.CreateClient("Google");
        var response = await client.PostAsync("token", content);
        if (!response.IsSuccessStatusCode)
        {
            throw new AuthenticationException("Failed to exchange authorization code.");
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();

        if (tokenResponse == null)
        {
            throw new InvalidOperationException("Failed to deserialize Google token response.");
        }

        return tokenResponse;
    }

    public async Task<GoogleUserInfo> GetUserInfoAsync(string accessToken)
    {
        var client = httpClientFactory.CreateClient("GoogleOpenId");
        
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync("v1/userinfo");
        if (!response.IsSuccessStatusCode)
        {
            throw new AuthenticationException("Failed to fetch user info.");
        }

        var userInfo = await response.Content.ReadFromJsonAsync<GoogleUserInfo>()
                       ?? throw new InvalidOperationException(
                           "Failed to deserialize Google user info.");
        
        return userInfo;
    }
}