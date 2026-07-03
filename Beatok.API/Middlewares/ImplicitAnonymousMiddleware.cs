using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Beatok.API.Attributes;
using Beatok.Application.Interfaces.Services;

namespace Beatok.API.Middlewares;

public class ImplicitAnonymousMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, 
        IAuthService authService,
        IUserService userService)
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
            else
            {
                var handler = new JwtSecurityTokenHandler();
                if (handler.ReadToken(context.Request.Cookies["jwt"]) is JwtSecurityToken jwt)
                {
                    var isAnonymous = jwt.Claims.FirstOrDefault(c => c.Type == "is_anonymous")?.Value == "true";

                    if (isAnonymous)
                    {
                        var userId = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                        if (Guid.TryParse(userId, out var id))
                        {
                            await userService.UpdateLastActiveAtAsync(id);
                        }
                    }
                }
            }
        }
        await next(context);
    }
}