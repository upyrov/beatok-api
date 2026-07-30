using Beatok.Application.Interfaces.Services;
using Beatok.Application.Mappings;
using Beatok.Application.Services;
using Beatok.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Beatok.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(UserSignupDtoValidator).Assembly);
        
        services.AddAutoMapper(cfg => {}, typeof(UserProfile));
        
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IKitService, KitService>();
        services.AddScoped<ILobbyService, LobbyService>();
        services.AddScoped<IScoreService, ScoreService>();
        services.AddScoped<ISoundService, SoundService>();
        services.AddScoped<ISubmissionService, SubmissionService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMmrService, MmrService>();
        
        return services;
    }
}