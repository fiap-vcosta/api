using Application.UseCases.Administrativo.Veiculo.Commands.DeleteVeiculo;
using Domain.Exceptions;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Administrativo.Veiculo.Commands.DeleteVeiculo;

public class DeleteVeiculoCommandHandlerTests
{
    private readonly Mock<IVeiculoGateway> _mockVeiculoGateway;
    private readonly DeleteVeiculoCommandHandler _handler;

    public DeleteVeiculoCommandHandlerTests()
    {
        _mockVeiculoGateway = new Mock<IVeiculoGateway>();
        _handler = new DeleteVeiculoCommandHandler(_mockVeiculoGateway.Object);
    }

    [Fact]
    public async Task Handle_DeletesVeiculo_WhenVeiculoExists()
    {
        _mockVeiculoGateway.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new VeiculoAggregateRoot { Id = 1, Placa = "ABC-1D23", IdCliente = 1, Modelo = "Gol", Marca = "Volkswagen" });
        _mockVeiculoGateway.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        await _handler.Handle(new DeleteVeiculoCommand { Id = 1 }, CancellationToken.None);

        _mockVeiculoGateway.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsDomainNotFoundException_WhenVeiculoDoesNotExist()
    {
        _mockVeiculoGateway.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((VeiculoAggregateRoot?)null);

        await Assert.ThrowsAsync<DomainNotFoundException>(() => _handler.Handle(new DeleteVeiculoCommand { Id = 999 }, CancellationToken.None));
    }
}
