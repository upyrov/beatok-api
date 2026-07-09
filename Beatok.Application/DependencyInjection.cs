using Beatok.Application.Interfaces.Services;
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
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<ILobbyService, LobbyService>();       
        
        return services;
    }
}