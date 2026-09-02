using Application.UseCases.Administrativo.Cliente.Queries.GetClienteById;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Administrativo.Cliente.Queries.GetClienteById;

public class GetClienteByIdQueryHandlerTests
{
    private readonly Mock<IClienteGateway> _mockClienteGateway = new();
    private readonly Mock<IVeiculoGateway> _mockVeiculoGateway = new();

    [Fact]
    public async Task Handle_ReturnsClienteWithVeiculos_WhenClienteExists()
    {
        // Arrange
        var cliente = new ClienteAggregateRoot { Id = 1, Nome = "Cliente Test", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" };
        var veiculos = new List<VeiculoAggregateRoot>
        {
            new() { Id = 10, Placa = "ABC-1D23", IdCliente = 1, Modelo = "Gol", Marca = "Volkswagen" }
        };

        _mockClienteGateway.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(cliente);
        _mockVeiculoGateway.Setup(r => r.GetByClienteIdAsync(1))
            .ReturnsAsync(veiculos);

        var handler = new GetClienteByIdQueryHandler(_mockClienteGateway.Object, _mockVeiculoGateway.Object);
        var query = new GetClienteByIdQuery { Id = 1 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Cliente Test", result.Nome);
        Assert.Equal(TipoDocumento.Cpf, result.TipoDocumento);
        Assert.Single(result.Veiculos);
        Assert.Equal(10, result.Veiculos[0].Id);
        Assert.Equal("ABC-1D23", result.Veiculos[0].Placa);
    }

    [Fact]
    public async Task Handle_ReturnsClienteWithEmptyVeiculos_WhenClienteHasNoVeiculos()
    {
        // Arrange
        var cliente = new ClienteAggregateRoot { Id = 1, Nome = "Cliente Test", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" };

        _mockClienteGateway.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(cliente);
        _mockVeiculoGateway.Setup(r => r.GetByClienteIdAsync(1))
            .ReturnsAsync([]);

        var handler = new GetClienteByIdQueryHandler(_mockClienteGateway.Object, _mockVeiculoGateway.Object);
        var query = new GetClienteByIdQuery { Id = 1 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Veiculos);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenClienteDoesNotExist()
    {
        // Arrange
        _mockClienteGateway.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((ClienteAggregateRoot?)null);

        var handler = new GetClienteByIdQueryHandler(_mockClienteGateway.Object, _mockVeiculoGateway.Object);
        var query = new GetClienteByIdQuery { Id = 999 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
