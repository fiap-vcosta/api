using Api.Extensions;
using Application;
using Application.Services;
using Infrastructure;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();
app.UseApiConfiguration();

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
