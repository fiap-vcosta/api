using Api.Extensions;
using Api.Logging;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Formatting.Compact;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog(DatadogLogging.Configure);

    builder.Services.AddApiServices(builder.Configuration);

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);
    });

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
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação encerrada inesperadamente");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program;
