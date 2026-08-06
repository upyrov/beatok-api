using System.Security.Claims;
using Beatok.Application.Interfaces.Services;

namespace Beatok.API.Middlewares;

public class AnonymousActivityMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IUserService userService)
    {
        if (context.User.Identity?.IsAuthenticated == true &&
            context.User.HasClaim(c => c.Type == "isAnonymous" &&
                                       c.Value == "true"))
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                await userService.UpdateLastActiveAtAsync(userId);
            }
        }
        await next(context);
    }
}