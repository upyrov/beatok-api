using Beatok.Application.Interfaces;
using Beatok.Application.Services;
using Beatok.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Beatok.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(UserRegisterDtoValidator).Assembly);
        
        services.AddScoped<IAuthService, AuthService>();
        
        return services;
    }
}