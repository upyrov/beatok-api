using System.Security.Claims;
using Beatok.Application.Interfaces.Services;

namespace Beatok.API.Middlewares;

public class AnonymousActivityMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IUserService userService)
    {
        if (context.User.Identity?.IsAuthenticated == true &&
            context.User.HasClaim(c => c.Type == "is_anonymous" &&
                                       c.Value == "true"))
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await userService.UpdateLastActiveAtAsync(userId);
        }
        await next(context);
    }
}