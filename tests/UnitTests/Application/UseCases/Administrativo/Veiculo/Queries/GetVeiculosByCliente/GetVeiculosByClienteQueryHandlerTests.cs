using Application.UseCases.Administrativo.Veiculo.Queries.GetVeiculosByCliente;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Administrativo.Veiculo.Queries.GetVeiculosByCliente;

public class GetVeiculosByClienteQueryHandlerTests
{
    private readonly Mock<IVeiculoGateway> _mockGateway = new();

    [Fact]
    public async Task Handle_ReturnsVeiculos_ForClienteId()
    {
        // Arrange
        var veiculos = new List<VeiculoAggregateRoot>
        {
            new() { Id = 1, Placa = "ABC-1D23", IdCliente = 1, Modelo = "Gol", Marca = "Volkswagen" },
            new() { Id = 2, Placa = "ABD-3F45", IdCliente = 1, Modelo = "Fox", Marca = "Volkswagen" }
        };

        _mockGateway.Setup(r => r.GetByClienteIdAsync(1)).ReturnsAsync(veiculos);

        var handler = new GetVeiculosByClienteQueryHandler(_mockGateway.Object);
        var query = new GetVeiculosByClienteQuery { IdCliente = 1 };

        // Act
        var result = (await handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Equal(1, r.IdCliente));
    }
}
