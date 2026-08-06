using System.Security.Claims;
using Beatok.Application.Interfaces.Services;

namespace Beatok.API.Middlewares;

public class UserProvisioningMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IUserService userService)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await next(context);
            return;
        }

        var firebaseUid = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(firebaseUid))
        {
            await next(context);
            return;
        }

        var name = context.User.FindFirst("name")?.Value ?? string.Empty;
        bool isAnonymous =
            context.User.FindFirst("provider_id")?.Value == "anonymous";

        await userService.EnsureExistsAsync(firebaseUid, name, isAnonymous);

        await next(context);
    }
}