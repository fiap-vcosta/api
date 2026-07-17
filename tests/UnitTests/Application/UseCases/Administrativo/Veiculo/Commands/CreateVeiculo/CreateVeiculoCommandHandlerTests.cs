using Application.UseCases.Administrativo.Veiculo.Commands.CreateVeiculo;
using Domain.Exceptions;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Administrativo.Veiculo.Commands.CreateVeiculo;

public class CreateVeiculoCommandHandlerTests
{
    private readonly Mock<IClienteGateway> _mockClienteGateway;
    private readonly Mock<IVeiculoGateway> _mockVeiculoGateway;
    private readonly CreateVeiculoCommandHandler _handler;

    public CreateVeiculoCommandHandlerTests()
    {
        _mockClienteGateway = new Mock<IClienteGateway>();
        _mockVeiculoGateway = new Mock<IVeiculoGateway>();
        _handler = new CreateVeiculoCommandHandler(_mockClienteGateway.Object, _mockVeiculoGateway.Object);
    }

    [Fact]
    public async Task Handle_CreatesVeiculo_WhenCommandIsValid()
    {
        var command = new CreateVeiculoCommand
        {
            Placa = "ABC-1D23",
            IdDono = 1,
            Modelo = "Gol",
            Marca = "Volkswagen"
        };

        _mockClienteGateway.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new ClienteAggregateRoot { Id = 1, Nome = "Cliente Teste", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" });

        _mockVeiculoGateway.Setup(r => r.GetByPlacaAsync(command.Placa))
            .ReturnsAsync((VeiculoAggregateRoot?)null);

        _mockVeiculoGateway.Setup(r => r.CreateAsync(It.IsAny<VeiculoAggregateRoot>()))
            .Callback<VeiculoAggregateRoot>(v => v.Id = 1)
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("ABC-1D23", result.Placa);
        Assert.Equal(1, result.IdDono);
        Assert.Equal("Gol", result.Modelo);
        Assert.Equal("Volkswagen", result.Marca);
        _mockVeiculoGateway.Verify(r => r.CreateAsync(It.IsAny<VeiculoAggregateRoot>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsDomainNotFoundException_WhenDonoDoesNotExist()
    {
        var command = new CreateVeiculoCommand { Placa = "ABC-1D23", IdDono = 999, Modelo = "Gol", Marca = "Volkswagen" };
        _mockClienteGateway.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ClienteAggregateRoot?)null);

        await Assert.ThrowsAsync<DomainNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ThrowsBusinessRuleException_WhenPlacaAlreadyExists()
    {
        var command = new CreateVeiculoCommand { Placa = "ABC-1D23", IdDono = 1, Modelo = "Gol", Marca = "Volkswagen" };
        _mockClienteGateway.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new ClienteAggregateRoot { Id = 1, Nome = "Cliente Teste", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" });
        _mockVeiculoGateway.Setup(r => r.GetByPlacaAsync(command.Placa))
            .ReturnsAsync(new VeiculoAggregateRoot { Id = 2, Placa = command.Placa, IdDono = 1, Modelo = "Uno", Marca = "Fiat" });

        await Assert.ThrowsAsync<BusinessRuleException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
