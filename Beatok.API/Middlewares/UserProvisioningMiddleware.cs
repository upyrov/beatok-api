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

            if (!string.IsNullOrEmpty(firebaseUid))
            {
                var userExists = await userService.ExistsAsync(firebaseUid);

                if (!userExists)
                {
                    var email = context.User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
                    var name = context.User.FindFirst("name")?.Value ?? string.Empty;
                    bool isAnonymous = bool.Parse(context.User.FindFirst(ClaimTypes.Anonymous)?.Value ?? "false");

                    await userService.CreateAsync(firebaseUid, name, isAnonymous, email);
                }
            }
        }
        await next(context);
    }
}