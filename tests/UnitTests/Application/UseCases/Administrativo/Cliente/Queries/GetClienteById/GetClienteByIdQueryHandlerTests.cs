using Application.UseCases.Administrativo.Cliente.Queries.GetClienteById;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Administrativo.Cliente.Queries.GetClienteById;

public class GetClienteByIdQueryHandlerTests
{
    private readonly Mock<IClienteGateway> _mockGateway = new();

    [Fact]
    public async Task Handle_ReturnsCliente_WhenClienteExists()
    {
        // Arrange
        var cliente = new ClienteAggregateRoot { Id = 1, Nome = "Cliente Test", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" };
        _mockGateway.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(cliente);

        var handler = new GetClienteByIdQueryHandler(_mockGateway.Object);
        var query = new GetClienteByIdQuery { Id = 1 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Cliente Test", result.Nome);
        Assert.Equal(TipoDocumento.Cpf, result.TipoDocumento);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenClienteDoesNotExist()
    {
        // Arrange
        _mockGateway.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((ClienteAggregateRoot?)null);

        var handler = new GetClienteByIdQueryHandler(_mockGateway.Object);
        var query = new GetClienteByIdQuery { Id = 999 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
