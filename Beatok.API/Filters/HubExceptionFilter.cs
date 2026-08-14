using Microsoft.AspNetCore.SignalR;
using Beatok.Application.Exceptions;

namespace Beatok.API.Filters;

public class HubExceptionFilter: IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, 
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        try
        {
            return await next(invocationContext);
        }
        catch (NotFoundException ex)
        {
            throw new HubException(ex.Message);
        }
        catch (BadRequestException ex)
        {
            throw new HubException(ex.Message);
        }
        catch (Exception)
        {
            throw new HubException("Something went wrong");
        }
    }
}