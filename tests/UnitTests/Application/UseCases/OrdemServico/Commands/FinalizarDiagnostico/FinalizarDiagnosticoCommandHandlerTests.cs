using Application.Abstractions.Events;
using Application.UseCases.OrdemServico.Commands.FinalizarDiagnostico;
using Application.Abstractions.Gateways;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Events;
using Domain.OrdemServico.ValueObjects;
using MediatR;
using Moq;

namespace UnitTests.Application.UseCases.OrdemServico.Commands.FinalizarDiagnostico;

public class FinalizarDiagnosticoCommandHandlerTests
{
    [Fact]
    public async Task Handle_FinalizesWithSuggestedServices()
    {
        // Arrange
        var mockOrdemServicoGateway = new Mock<IOrdemServicoGateway>();
        var mockMediator = new Mock<IMediator>();

        var cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" };
        var veiculo = new VeiculoOrdemServico { Placa = "XYZ-9876", Marca = "Toyota", Modelo = "Corolla" };
        var ordemServico = OrdemServicoAggregateRoot.Criar(cliente, veiculo);
        ordemServico.EnviarParaDiagnostico();
        
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        ordemServico.AdicionarItemServico("Troca", 100m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>());

        var command = new FinalizarDiagnosticoCommand { IdOrdemServico = 1 };

        mockOrdemServicoGateway.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ordemServico);
        mockOrdemServicoGateway.Setup(r => r.UpdateAsync(It.IsAny<OrdemServicoAggregateRoot>())).Returns(Task.CompletedTask);
        mockMediator.Setup(m => m.Publish(It.IsAny<DomainEventNotification<DiagnosticoPreenchidoEvent>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new FinalizarDiagnosticoCommandHandler(mockOrdemServicoGateway.Object, mockMediator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusOrdemServico.AguardandoAprovacao, result.Status);
    }
}
