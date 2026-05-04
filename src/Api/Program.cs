using Api.Extensions;
using Api.Middlewares;
using Application.Services;
using Domain.Admin;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();
app.UseApiConfiguration();

app.UseWhen(context => context.Request.Path.Equals("/health", StringComparison.OrdinalIgnoreCase), cfg =>
{
    cfg.UseMiddleware<UserRoleValidationMiddleware>(new[] { TipoUsuario.Admin });
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapGet("/health", async (IHealthService healthService) =>
{
    var isDbOk = await healthService.CheckDatabaseAsync();
    return isDbOk ? Results.Ok("OK") : Results.StatusCode(503);
});

app.Run();
