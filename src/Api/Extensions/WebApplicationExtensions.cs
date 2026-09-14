using Api.Middleware;
using Serilog;
using Serilog.Events;

namespace Api.Extensions;

public static class WebApplicationExtensions
{
    public static void UseApiConfiguration(this WebApplication app)
    {
        app.UseMiddleware<RequestIdMiddleware>();
        app.UseSerilogRequestLogging(options =>
        {
            options.GetLevel = (httpContext, _, exception) =>
            {
                if (exception is not null)
                {
                    return LogEventLevel.Error;
                }

                return httpContext.Request.Path.StartsWithSegments("/health")
                    ? LogEventLevel.Debug
                    : LogEventLevel.Information;
            };
        });
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}
