using Application.UseCases.Administrativo.Cliente.Queries.GetAllClientes;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Administrativo.Cliente.Queries.GetAllClientes;

public class GetAllClientesQueryHandlerTests
{
    private readonly Mock<IClienteGateway> _mockGateway = new();

    [Fact]
    public async Task Handle_ReturnsAllClientes()
    {
        // Arrange
        var clientes = new List<ClienteAggregateRoot>
        {
            new() { Id = 1, Nome = "Cliente 1", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" },
            new() { Id = 2, Nome = "Cliente 2", TipoDocumento = TipoDocumento.Cnpj, Documento = "12345678901234" }
        };

        _mockGateway.Setup(r => r.GetAllAsync())
            .ReturnsAsync(clientes);

        var handler = new GetAllClientesQueryHandler(_mockGateway.Object);
        var query = new GetAllClientesQuery();

        // Act
        var result = (await handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Nome == "Cliente 1");
        Assert.Contains(result, c => c.Nome == "Cliente 2");
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoClientesExist()
    {
        // Arrange
        _mockGateway.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<ClienteAggregateRoot>());

        var handler = new GetAllClientesQueryHandler(_mockGateway.Object);
        var query = new GetAllClientesQuery();

        // Act
        var result = (await handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
