using Beatok.API.Attributes;
using Beatok.Application.Interfaces.Services;

namespace Beatok.API.Middlewares;

public class ImplicitAnonymousMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IAuthService authService)
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
            
                context.Response.Cookies.Append("jwt", authResult.Token, new CookieOptions
                {
                    HttpOnly = true, 
                    Secure = true,   
                    Expires = authResult.Expires,
                    SameSite = SameSiteMode.Strict
                });
            
                context.Request.Headers.Append("Authorization", $"Bearer {authResult.Token}");
            }
        }
        
        await next(context);
    }
}