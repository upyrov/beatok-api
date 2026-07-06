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
                var authResult = await authService.LoginAnonymousAsync();

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = authResult.Expires,
                    SameSite = SameSiteMode.Strict
                };
            
                context.Response.Cookies.Append("jwt", authResult.AccessToken, cookieOptions);
                context.Response.Cookies.Append("refresh_token", authResult.RefreshToken, cookieOptions);
            
                context.Request.Headers.Append("Authorization", $"Bearer {authResult.AccessToken}");
            }
        }
        await next(context);
    }
}