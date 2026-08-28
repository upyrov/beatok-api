using System.Security.Authentication;
using Beatok.Application.DTOs;
using Beatok.Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace Beatok.API.ExceptionHandling;

public class GlobalExceptionHandler: IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context,
        Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            InvalidCredentialException => StatusCodes.Status401Unauthorized,
            NotFoundException => StatusCodes.Status404NotFound,
            TokenExpiredException => StatusCodes.Status401Unauthorized,
            BusinessException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
        context.Response.StatusCode = statusCode;
        
        if (exception is ValidationException validationException)
        {
            await context.Response.WriteAsJsonAsync(new ErrorDto
            {
                Message = validationException.Errors.First().ErrorMessage,
                StatusCode = statusCode
            });
            
            return true;
        }

        await context.Response.WriteAsJsonAsync(new ErrorDto
        {
            Message = exception.Message,
            StatusCode = statusCode
        });

        return true;
    }
}