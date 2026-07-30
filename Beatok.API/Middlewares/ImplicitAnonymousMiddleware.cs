using Beatok.API.Attributes;
using Beatok.Application.Interfaces.Services;

namespace Beatok.API.Middlewares;

public class ImplicitAnonymousMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, 
        IAuthService authService)
    {
        var endpoint = context.GetEndpoint();

        var hasAttribute = endpoint?.Metadata
            .GetMetadata<ImplicitAnonymousAttribute>() != null;

        if (hasAttribute)
        {
            var hasToken = context.Request.Cookies.ContainsKey("jwt");
            if (!hasToken)
            {
                var AuthResultDto = await authService.SignInAnonymousAsync();

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = AuthResultDto.Expires,
                    SameSite = SameSiteMode.None
                };
            
                context.Response.Cookies.Append("jwt", AuthResultDto.AccessToken, cookieOptions);
                context.Response.Cookies.Append("refresh_token", AuthResultDto.RefreshToken, cookieOptions);
            
                context.Request.Headers.Append("Authorization", $"Bearer {AuthResultDto.AccessToken}");
            }
        }
        await next(context);
    }
}