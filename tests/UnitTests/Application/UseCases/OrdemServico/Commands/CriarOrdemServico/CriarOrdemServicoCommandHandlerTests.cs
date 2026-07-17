using Application.Abstractions.Events;
using Application.UseCases.OrdemServico.Commands.CriarOrdemServico;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Events;
using MediatR;
using Moq;

namespace UnitTests.Application.UseCases.OrdemServico.Commands.CriarOrdemServico;

public class CriarOrdemServicoCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreatesOrdemServico_WhenCommandIsValid()
    {
        // Arrange
        var mockVeiculoGateway = new Mock<IVeiculoGateway>();
        var mockClienteGateway = new Mock<IClienteGateway>();
        var mockOrdemServicoGateway = new Mock<IOrdemServicoGateway>();
        var mockMediator = new Mock<IMediator>();

        var cliente = new ClienteAggregateRoot { Id = 1, Nome = "João", Email = "joao@test.com", TipoDocumento = TipoDocumento.Cpf, Documento = "12345678901" };
        var veiculo = new VeiculoAggregateRoot { Id = 1, Placa = "ABC-1234", Marca = "Toyota", Modelo = "Corolla", IdDono = 1 };
        
        var command = new CriarOrdemServicoCommand { IdVeiculo = 1 };

        mockVeiculoGateway.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(veiculo);
        mockClienteGateway.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cliente);
        mockOrdemServicoGateway.Setup(r => r.CriarAsync(It.IsAny<OrdemServicoAggregateRoot>())).Returns(Task.CompletedTask);
        mockMediator.Setup(m => m.Publish(It.IsAny<DomainEventNotification<OrdemServicoCriadaEvent>>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new CriarOrdemServicoCommandHandler(
            mockVeiculoGateway.Object,
            mockClienteGateway.Object,
            mockOrdemServicoGateway.Object,
            mockMediator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cliente.Nome, result.Cliente.Nome);
        Assert.Equal(veiculo.Placa, result.Veiculo.Placa);
        mockOrdemServicoGateway.Verify(r => r.CriarAsync(It.IsAny<OrdemServicoAggregateRoot>()), Times.Once);
        mockMediator.Verify(m => m.Publish(It.IsAny<DomainEventNotification<OrdemServicoCriadaEvent>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
