using Application.UseCases.Administrativo.Cliente.Commands.DeleteCliente;
using Domain.Exceptions;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Administrativo.Cliente.Commands.DeleteCliente;

public class DeleteClienteCommandHandlerTests
{
    private readonly Mock<IClienteGateway> _mockGateway;
    private readonly DeleteClienteCommandHandler _handler;

    public DeleteClienteCommandHandlerTests()
    {
        _mockGateway = new Mock<IClienteGateway>();
        _handler = new DeleteClienteCommandHandler(_mockGateway.Object);
    }

    [Fact]
    public async Task Handle_DeletesCliente_WhenClienteExists()
    {
        // Arrange
        var command = new DeleteClienteCommand { Id = 1 };
        var existingCliente = new ClienteAggregateRoot { Id = 1, Nome = "Cliente to Delete", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" };

        _mockGateway.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingCliente);

        _mockGateway.Setup(r => r.DeleteAsync(1))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockGateway.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsDomainNotFoundException_WhenClienteDoesNotExist()
    {
        // Arrange
        var command = new DeleteClienteCommand { Id = 999 };

        _mockGateway.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((ClienteAggregateRoot?)null);

        // Act & Assert
        await Assert.ThrowsAsync<DomainNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
