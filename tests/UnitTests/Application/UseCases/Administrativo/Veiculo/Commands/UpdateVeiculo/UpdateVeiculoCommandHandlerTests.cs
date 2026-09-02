using Application.UseCases.Administrativo.Veiculo.Commands.UpdateVeiculo;
using Domain.Exceptions;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Administrativo.Veiculo.Commands.UpdateVeiculo;

public class UpdateVeiculoCommandHandlerTests
{
    private readonly Mock<IClienteGateway> _mockClienteGateway;
    private readonly Mock<IVeiculoGateway> _mockVeiculoGateway;
    private readonly UpdateVeiculoCommandHandler _handler;

    public UpdateVeiculoCommandHandlerTests()
    {
        _mockClienteGateway = new Mock<IClienteGateway>();
        _mockVeiculoGateway = new Mock<IVeiculoGateway>();
        _handler = new UpdateVeiculoCommandHandler(_mockClienteGateway.Object, _mockVeiculoGateway.Object);
    }

    [Fact]
    public async Task Handle_UpdatesVeiculo_WhenVeiculoExists()
    {
        var command = new UpdateVeiculoCommand
        {
            Id = 1,
            Placa = "DEF-2G34",
            IdCliente = 1,
            Modelo = "Polo",
            Marca = "Volkswagen"
        };

        _mockVeiculoGateway.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new VeiculoAggregateRoot { Id = 1, Placa = "ABC-1D23", IdCliente = 1, Modelo = "Gol", Marca = "Volkswagen" });

        _mockClienteGateway.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new ClienteAggregateRoot { Id = 1, Nome = "Cliente Teste", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" });

        _mockVeiculoGateway.Setup(r => r.GetByPlacaAsync(command.Placa))
            .ReturnsAsync((VeiculoAggregateRoot?)null);

        _mockVeiculoGateway.Setup(r => r.UpdateAsync(It.IsAny<VeiculoAggregateRoot>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("DEF-2G34", result.Placa);
        Assert.Equal("Polo", result.Modelo);
        _mockVeiculoGateway.Verify(r => r.UpdateAsync(It.IsAny<VeiculoAggregateRoot>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsDomainNotFoundException_WhenVeiculoDoesNotExist()
    {
        var command = new UpdateVeiculoCommand { Id = 999, Placa = "DEF-2G34", IdCliente = 1, Modelo = "Polo", Marca = "Volkswagen" };
        _mockVeiculoGateway.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((VeiculoAggregateRoot?)null);

        await Assert.ThrowsAsync<DomainNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ThrowsBusinessRuleException_WhenAnotherVeiculoHasSamePlaca()
    {
        var command = new UpdateVeiculoCommand { Id = 1, Placa = "DEF-2G34", IdCliente = 1, Modelo = "Polo", Marca = "Volkswagen" };

        _mockVeiculoGateway.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new VeiculoAggregateRoot { Id = 1, Placa = "ABC-1D23", IdCliente = 1, Modelo = "Gol", Marca = "Volkswagen" });

        _mockClienteGateway.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new ClienteAggregateRoot { Id = 1, Nome = "Cliente Teste", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" });

        _mockVeiculoGateway.Setup(r => r.GetByPlacaAsync(command.Placa))
            .ReturnsAsync(new VeiculoAggregateRoot { Id = 2, Placa = command.Placa, IdCliente = 1, Modelo = "Uno", Marca = "Fiat" });

        await Assert.ThrowsAsync<BusinessRuleException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
