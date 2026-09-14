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
            Log.Information("DD_API_KEY ausente; sink Datadog de logs não será registrado");
            return;
        }

        var service = context.Configuration["DD_SERVICE"];
        if (string.IsNullOrWhiteSpace(service))
        {
            Log.Warning("DD_SERVICE ausente; sink Datadog de logs não será registrado");
            return;
        }

        var env = context.Configuration["DD_ENV"];
        if (string.IsNullOrWhiteSpace(env))
        {
            Log.Warning("DD_ENV ausente; sink Datadog de logs não será registrado");
            return;
        }

        var intakeUrl = context.Configuration["Datadog:LogsIntakeUrl"];
        if (string.IsNullOrWhiteSpace(intakeUrl))
        {
            Log.Warning("Datadog:LogsIntakeUrl ausente; sink Datadog de logs não será registrado");
            return;
        }

        configuration.WriteTo.DatadogLogs(
            apiKey,
            source: "csharp",
            service: service,
            host: Environment.MachineName,
            tags: [$"env:{env}"],
            configuration: new DatadogConfiguration { Url = intakeUrl });
    }
}
