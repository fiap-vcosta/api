using Application.Abstractions.Events;
using Application.Abstractions.Services;
using Application.UseCases.OrdemServico.Policies;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Events;
using Moq;

namespace UnitTests.Application.UseCases.OrdemServico.Policies;

public class OrdemServicoMetricsPolicyTests
{
    [Fact]
    public async Task Handle_OrdemServicoStatusAlterado_IncrementsStatus()
    {
        // Arrange
        var metrics = new Mock<IOsMetrics>();
        var policy = new OrdemServicoMetricsPolicy(metrics.Object);
        var domainEvent = new OrdemServicoStatusAlteradoEvent(42, StatusOrdemServico.EmDiagnostico);

        // Act
        await policy.Handle(new DomainEventNotification<OrdemServicoStatusAlteradoEvent>(domainEvent), CancellationToken.None);

        // Assert
        metrics.Verify(m => m.IncrementStatus(42, StatusOrdemServico.EmDiagnostico), Times.Once);
    }
}
