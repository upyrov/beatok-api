using Beatok.API.ExceptionHandling;
using Beatok.API.Extensions;
using Beatok.API.Hubs;
using Beatok.API.Middlewares;
using Beatok.API.Notifications;
using Beatok.Application;
using Beatok.Application.Interfaces;
using Beatok.Infrastructure;
using Beatok.Infrastructure.Authentication;
using Hangfire;
using Beatok.Infrastructure.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var jwtOptions = builder.Configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
builder.Services.AddApiAuthentication(jwtOptions!);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ILobbyNotifier, SignalRLobbyNotifier>();

builder.Services.AddMemoryCache();

builder.Services.AddControllers();
builder.Services.AddSignalR();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DatabaseInitializer.SeedAsync(services);
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseMiddleware<ImplicitAnonymousMiddleware>();

app.UseAuthentication();

app.UseMiddleware<AnonymousActivityMiddleware>();

app.UseAuthorization();

app.UseHangfireDashboard();

app.MapControllers();
app.MapHub<LobbyHub>("/lobby");

app.Run();