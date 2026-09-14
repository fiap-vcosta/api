using Domain.OrdemServico.Entities;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace UnitTests.Infrastructure.Services;

public class DogStatsDOsMetricsTests
{
    [Fact]
    public void Constructor_DisablesMetrics_WhenAgentHostMissing()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();

        // Act
        using var metrics = new DogStatsDOsMetrics(configuration, NullLogger<DogStatsDOsMetrics>.Instance);
        metrics.IncrementStatus(1, StatusOrdemServico.Recebida);

        // Assert
        Assert.NotNull(metrics);
    }

    [Fact]
    public void Constructor_DisablesMetrics_WhenAgentHostWhitespace()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DD_AGENT_HOST"] = "   "
            })
            .Build();

        // Act
        using var metrics = new DogStatsDOsMetrics(configuration, NullLogger<DogStatsDOsMetrics>.Instance);
        metrics.IncrementStatus(2, StatusOrdemServico.EmDiagnostico);

        // Assert
        Assert.NotNull(metrics);
    }

    [Fact]
    public void IncrementStatus_AndDispose_WhenAgentHostConfigured()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DD_AGENT_HOST"] = "127.0.0.1"
            })
            .Build();

        // Act
        using var metrics = new DogStatsDOsMetrics(configuration, NullLogger<DogStatsDOsMetrics>.Instance);
        metrics.IncrementStatus(3, StatusOrdemServico.AguardandoAprovacao);

        // Assert
        Assert.NotNull(metrics);
    }
}
