using Beatok.API.ExceptionHandling;
using Beatok.API.Extensions;
using Beatok.API.Middlewares;
using Beatok.Application;
using Beatok.Infrastructure;
using Beatok.Infrastructure.Authentication;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var jwtOptions = builder.Configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
builder.Services.AddApiAuthentication(jwtOptions!);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseMiddleware<ImplicitAnonymousMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();