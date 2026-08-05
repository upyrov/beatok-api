using System.Text;
using Beatok.Domain.Entities;
using Beatok.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Beatok.API.Extensions;

public static class ApiExtensions
{
    public static void AddApiAuthentication(this IServiceCollection services,
        FirebaseOptions firebaseOptions)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = $"https://securetoken.google.com/{firebaseOptions.ProjectId}";
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidAudience = $"{firebaseOptions.ProjectId}",
                    ValidIssuer = $"https://securetoken.google.com/{firebaseOptions.ProjectId}"            
                };
            });

        services.AddAuthorization(options =>
            options.AddPolicy("OnlyAdmin", policy =>
                policy.RequireRole(nameof(UserRole.Administrator))));
    }
}