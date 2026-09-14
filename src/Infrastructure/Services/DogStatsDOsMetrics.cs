using Application.Abstractions.Services;
using Domain.OrdemServico.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StatsdClient;

namespace Infrastructure.Services;

public sealed partial class DogStatsDOsMetrics : IOsMetrics, IDisposable
{
    private readonly bool _configured;

    public DogStatsDOsMetrics(IConfiguration configuration, ILogger<DogStatsDOsMetrics> logger)
    {
        var agentHost = configuration["DD_AGENT_HOST"];
        if (string.IsNullOrWhiteSpace(agentHost))
        {
            LogAgentHostAusente(logger);
            _configured = false;
            return;
        }

        _configured = DogStatsd.Configure(new StatsdConfig
        {
            StatsdServerName = agentHost,
            StatsdPort = 8125,
            Prefix = "techchallenge"
        });

        if (!_configured)
        {
            LogFalhaConfigurarDogStatsD(logger, agentHost);
        }
    }

    public void IncrementStatus(int idOrdemServico, StatusOrdemServico status)
    {
        if (!_configured)
        {
            return;
        }

        DogStatsd.Increment(
            "ordem_servico.status",
            tags: [$"status:{status}", $"ordem_servico_id:{idOrdemServico}"]);
    }

    public void Dispose()
    {
        if (_configured)
        {
            DogStatsd.Dispose();
        }
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "DD_AGENT_HOST ausente; métricas DogStatsD de OS desabilitadas")]
    private static partial void LogAgentHostAusente(ILogger logger);

    [LoggerMessage(EventId = 2, Level = LogLevel.Warning, Message = "Falha ao configurar DogStatsD em {AgentHost}; métricas de OS desabilitadas")]
    private static partial void LogFalhaConfigurarDogStatsD(ILogger logger, string agentHost);
}
