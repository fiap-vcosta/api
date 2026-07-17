using Application.UseCases.Administrativo.Cliente.Commands.CreateCliente;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Administrativo.Cliente.Commands.CreateCliente;

public class CreateClienteCommandHandlerTests
{
    private readonly Mock<IClienteGateway> _mockGateway;
    private readonly CreateClienteCommandHandler _handler;

    public CreateClienteCommandHandlerTests()
    {
        _mockGateway = new Mock<IClienteGateway>();
        _handler = new CreateClienteCommandHandler(_mockGateway.Object);
    }

    [Fact]
    public async Task Handle_CreatesCliente_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateClienteCommand
        {
            Nome = "Cliente Teste",
            TipoDocumento = TipoDocumento.Cpf,
            Documento = "11144477735"
        };

        _mockGateway.Setup(r => r.CreateAsync(It.IsAny<ClienteAggregateRoot>()))
            .Callback<ClienteAggregateRoot>(c => c.Id = 1)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Cliente Teste", result.Nome);
        Assert.Equal(TipoDocumento.Cpf, result.TipoDocumento);
        Assert.Equal("11144477735", result.Documento);
        _mockGateway.Verify(r => r.CreateAsync(It.IsAny<ClienteAggregateRoot>()), Times.Once);
    }
}
