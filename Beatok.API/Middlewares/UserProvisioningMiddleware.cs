using System.Security.Claims;
using Beatok.Application.Interfaces.Services;

namespace Beatok.API.Middlewares;

public class UserProvisioningMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IUserService userService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var firebaseUid = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var name = context.User.FindFirst("name")?.Value ?? string.Empty;
            bool isAnonymous = bool.Parse(context.User.FindFirst("isAnonymous")?.Value ?? "false"); 
            await userService.EnsureExistsAsync(firebaseUid, name, isAnonymous);
        }
        await next(context);
    }
}