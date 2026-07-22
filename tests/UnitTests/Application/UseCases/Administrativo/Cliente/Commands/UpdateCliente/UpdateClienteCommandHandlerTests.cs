using Application.UseCases.Administrativo.Cliente.Commands.UpdateCliente;
using Domain.Exceptions;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Administrativo.Cliente.Commands.UpdateCliente;

public class UpdateClienteCommandHandlerTests
{
    private readonly Mock<IClienteGateway> _mockGateway;
    private readonly UpdateClienteCommandHandler _handler;

    public UpdateClienteCommandHandlerTests()
    {
        _mockGateway = new Mock<IClienteGateway>();
        _handler = new UpdateClienteCommandHandler(_mockGateway.Object);
    }

    [Fact]
    public async Task Handle_UpdatesCliente_WhenClienteExists()
    {
        // Arrange
        var command = new UpdateClienteCommand
        {
            Id = 1,
            Nome = "Updated Cliente",
            TipoDocumento = TipoDocumento.Cnpj,
            Documento = "12345678901234"
        };

        var existingCliente = new ClienteAggregateRoot { Id = 1, Nome = "Old Name", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" };
        
        _mockGateway.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingCliente);

        _mockGateway.Setup(r => r.UpdateAsync(It.IsAny<ClienteAggregateRoot>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Updated Cliente", result.Nome);
        Assert.Equal(TipoDocumento.Cnpj, result.TipoDocumento);
        _mockGateway.Verify(r => r.UpdateAsync(It.IsAny<ClienteAggregateRoot>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsDomainNotFoundException_WhenClienteDoesNotExist()
    {
        // Arrange
        var command = new UpdateClienteCommand { Id = 999, Nome = "Test", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" };

        _mockGateway.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((ClienteAggregateRoot?)null);

        // Act & Assert
        await Assert.ThrowsAsync<DomainNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
