using Beatok.Application.Interfaces.Services;

namespace Beatok.API.Middlewares;

public class UserProvisioningMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IUserService userService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var firebaseUid = context.User.FindFirst("uid")?.Value;

            if (!string.IsNullOrEmpty(firebaseUid))
            {
                var userExists = await userService.ExistsAsync(firebaseUid);

                if (!userExists)
                {
                    var email = context.User.FindFirst("email")?.Value ?? string.Empty;
                    var name = context.User.FindFirst("displayName")?.Value ?? string.Empty;
                    var isAnonymous = context.User.HasClaim(c => c.Type == "IsAnonymous" && c.Value == "true");

                    await userService.CreateAsync(firebaseUid, name, isAnonymous, email);
                }
            }
        }
        await next(context);
    }
}