using Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using StatsdClient;

namespace Infrastructure.Services;

public sealed class DogStatsDOsMetrics : IOsMetrics, IDisposable
{
    private readonly bool _configured;

    public DogStatsDOsMetrics(IConfiguration configuration)
    {
        var agentHost = configuration["DD_AGENT_HOST"];
        if (string.IsNullOrWhiteSpace(agentHost))
        {
            agentHost = Environment.GetEnvironmentVariable("DD_AGENT_HOST");
        }

        if (string.IsNullOrWhiteSpace(agentHost))
        {
            agentHost = "127.0.0.1";
        }

        _configured = DogStatsd.Configure(new StatsdConfig
        {
            StatsdServerName = agentHost,
            StatsdPort = 8125,
            Prefix = "techchallenge"
        });
    }

    public void IncrementCriada() => Increment("ordem_servico.criada");

    public void IncrementEvento(string evento)
    {
        if (string.IsNullOrWhiteSpace(evento))
        {
            return;
        }

        Increment("ordem_servico.evento", $"evento:{evento}");
    }

    private void Increment(string metric, params string[] tags)
    {
        if (!_configured)
        {
            return;
        }

        DogStatsd.Increment(metric, tags: tags);
    }

    public void Dispose() => DogStatsd.Dispose();
}
