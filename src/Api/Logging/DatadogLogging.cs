using Serilog;
using Serilog.Sinks.Datadog.Logs;

namespace Api.Logging;

public static class DatadogLogging
{
    public static void Configure(HostBuilderContext context, IServiceProvider _, LoggerConfiguration configuration)
    {
        configuration.ReadFrom.Configuration(context.Configuration);

        var apiKey = context.Configuration["DD_API_KEY"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return;
        }

        var service = context.Configuration["DD_SERVICE"]
            ?? context.Configuration["Serilog:Properties:service"]
            ?? "api";
        var env = context.Configuration["DD_ENV"]
            ?? context.HostingEnvironment.EnvironmentName;
        var intakeUrl = context.Configuration["Datadog:LogsIntakeUrl"]
            ?? "https://http-intake.logs.datadoghq.com";

        configuration.WriteTo.DatadogLogs(
            apiKey,
            source: "csharp",
            service: service,
            host: Environment.MachineName,
            tags: [$"env:{env}"],
            configuration: new DatadogConfiguration { Url = intakeUrl });
    }
}
