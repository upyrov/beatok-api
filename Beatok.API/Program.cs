using Beatok.API.ExceptionHandling;
using Beatok.API.Extensions;
using Beatok.API.Hubs;
using Beatok.API.Middlewares;
using Beatok.API.Notifications;
using Beatok.Application;
using Beatok.Application.Interfaces;
using Beatok.Infrastructure;
using Hangfire;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;
using Beatok.API.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(builder.Configuration.GetValue<string>("FrontendUrl")!)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<AdminAuthorizationFilter>();

builder.Services.AddScoped<ILobbyNotifier, SignalRLobbyNotifier>();

builder.Services.Configure<FirebaseOptions>(
    builder.Configuration.GetSection("FirebaseOptions"));

var firebaseOptions = builder.Configuration.GetSection("FirebaseOptions").Get<FirebaseOptions>();
builder.Services.AddApiAuthentication(firebaseOptions!);

builder.Services.AddMemoryCache();

builder.Services.AddControllers();
builder.Services.AddSignalR();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? httpContext.TraceIdentifier;
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: clientIp,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });

    options.AddPolicy("LimitSignalR", httpContext =>
    {
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? httpContext.TraceIdentifier;
        // Partition traffic so every unique IP gets its own independent bucket
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: clientIp,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 50,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseForwardedHeaders();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseRateLimiter();

app.UseAuthentication();

app.UseMiddleware<UserProvisioningMiddleware>();
app.UseMiddleware<AnonymousActivityMiddleware>();

app.UseAuthorization();

app.UseHangfireDashboard();

app.MapControllers();
app.MapHub<LobbyHub>("/lobby").RequireRateLimiting("LimitSignalR");

app.Run();