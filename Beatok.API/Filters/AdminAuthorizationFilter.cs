using System.Security.Claims;
using Beatok.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Beatok.API.Filters;

public class AdminAuthorizationFilter(IUserService userService): IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var userId = context.HttpContext.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        if (!await userService.IsAdminAsync(userId))
        {
            context.Result = new ForbidResult();
        }        
    }
}