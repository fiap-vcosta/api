using Api.Extensions;
using Application.Abstractions.Services;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseApiConfiguration();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapGet("/health", async (AppDbContext appDbContext) =>
{
    var isDbOk = await appDbContext.Database.CanConnectAsync();
    return isDbOk ? Results.Ok("OK") : Results.StatusCode(503);
});

app.Run();

public partial class Program;
