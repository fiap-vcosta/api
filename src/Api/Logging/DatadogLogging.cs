using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Datadog.Logs;

namespace Api.Logging;

public static class DatadogLogging
{
    public static void Configure(HostBuilderContext context, IServiceProvider _, LoggerConfiguration configuration)
    {
        var service = Environment.GetEnvironmentVariable("DD_SERVICE") ?? "api";
        var env = Environment.GetEnvironmentVariable("DD_ENV")
            ?? context.HostingEnvironment.EnvironmentName;

        configuration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("service", service)
            .Enrich.WithProperty("env", env)
            .WriteTo.Console(new RenderedCompactJsonFormatter());

        var apiKey = context.Configuration["DD_API_KEY"]
            ?? Environment.GetEnvironmentVariable("DD_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return;
        }

        var site = context.Configuration["DD_SITE"]
            ?? Environment.GetEnvironmentVariable("DD_SITE")
            ?? "datadoghq.com";
        var intakeUrl = site.Equals("datadoghq.com", StringComparison.OrdinalIgnoreCase)
            ? "https://http-intake.logs.datadoghq.com"
            : $"https://http-intake.logs.{site}";

        configuration.WriteTo.DatadogLogs(
            apiKey,
            source: "csharp",
            service: service,
            host: Environment.MachineName,
            tags: [$"env:{env}"],
            configuration: new DatadogConfiguration { Url = intakeUrl });
    }
}
