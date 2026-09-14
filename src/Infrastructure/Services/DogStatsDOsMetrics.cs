using Application.Abstractions.Services;
using Domain.OrdemServico.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StatsdClient;

namespace Infrastructure.Services;

public sealed class DogStatsDOsMetrics : IOsMetrics, IDisposable
{
    private readonly bool _configured;

    public DogStatsDOsMetrics(IConfiguration configuration, ILogger<DogStatsDOsMetrics> logger)
    {
        var agentHost = configuration["DD_AGENT_HOST"];
        if (string.IsNullOrWhiteSpace(agentHost))
        {
            logger.LogWarning("DD_AGENT_HOST ausente; métricas DogStatsD de OS desabilitadas");
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
            logger.LogWarning("Falha ao configurar DogStatsD em {AgentHost}; métricas de OS desabilitadas", agentHost);
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
}
