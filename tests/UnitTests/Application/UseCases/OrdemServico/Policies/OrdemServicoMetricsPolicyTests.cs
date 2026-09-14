using Application.Abstractions.Events;
using Application.Abstractions.Services;
using Application.UseCases.OrdemServico.Policies;
using Domain.OrdemServico.Events;
using Moq;

namespace UnitTests.Application.UseCases.OrdemServico.Policies;

public class OrdemServicoMetricsPolicyTests
{
    [Fact]
    public async Task Handle_OrdemServicoCriada_IncrementsCriadaAndEvento()
    {
        // Arrange
        var metrics = new Mock<IOsMetrics>();
        var policy = new OrdemServicoMetricsPolicy(metrics.Object);

        // Act
        await policy.Handle(
            new DomainEventNotification<OrdemServicoCriadaEvent>(new OrdemServicoCriadaEvent(1)),
            CancellationToken.None);

        // Assert
        metrics.Verify(m => m.IncrementCriada(), Times.Once);
        metrics.Verify(m => m.IncrementEvento("criada"), Times.Once);
    }

    [Fact]
    public async Task Handle_OrdemServicoAprovada_IncrementsEvento()
    {
        // Arrange
        var metrics = new Mock<IOsMetrics>();
        var policy = new OrdemServicoMetricsPolicy(metrics.Object);

        // Act
        await policy.Handle(
            new DomainEventNotification<OrdemServicoAprovadaEvent>(new OrdemServicoAprovadaEvent(2)),
            CancellationToken.None);

        // Assert
        metrics.Verify(m => m.IncrementEvento("aprovada"), Times.Once);
        metrics.Verify(m => m.IncrementCriada(), Times.Never);
    }
}
