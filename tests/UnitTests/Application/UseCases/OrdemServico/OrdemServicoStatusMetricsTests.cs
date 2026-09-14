using Application.Abstractions.Services;
using Application.UseCases.OrdemServico;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.ValueObjects;
using Moq;

namespace UnitTests.Application.UseCases.OrdemServico;

public class OrdemServicoStatusMetricsTests
{
    [Fact]
    public void EmitIfChanged_Increments_WhenStatusChanged()
    {
        // Arrange
        var metrics = new Mock<IOsMetrics>();
        var ordem = OrdemServicoAggregateRoot.Criar(
            new ClienteOrdemServico { Id = 1, Nome = "A", Email = "a@t.com" },
            new VeiculoOrdemServico { Placa = "ABC1D23", Marca = "VW", Modelo = "Gol" });
        typeof(OrdemServicoAggregateRoot).GetProperty(nameof(OrdemServicoAggregateRoot.Id))!.SetValue(ordem, 7);
        var anterior = ordem.Status;
        ordem.EnviarParaDiagnostico();

        // Act
        OrdemServicoStatusMetrics.EmitIfChanged(metrics.Object, ordem, anterior);

        // Assert
        metrics.Verify(m => m.IncrementStatus(7, StatusOrdemServico.EmDiagnostico), Times.Once);
    }

    [Fact]
    public void EmitIfChanged_DoesNothing_WhenStatusUnchanged()
    {
        // Arrange
        var metrics = new Mock<IOsMetrics>();
        var ordem = OrdemServicoAggregateRoot.Criar(
            new ClienteOrdemServico { Id = 1, Nome = "A", Email = "a@t.com" },
            new VeiculoOrdemServico { Placa = "ABC1D23", Marca = "VW", Modelo = "Gol" });

        // Act
        OrdemServicoStatusMetrics.EmitIfChanged(metrics.Object, ordem, ordem.Status);

        // Assert
        metrics.Verify(m => m.IncrementStatus(It.IsAny<int>(), It.IsAny<StatusOrdemServico>()), Times.Never);
    }
}
