using Application.UseCases.Administrativo.Veiculo.Queries.GetAllVeiculos;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Administrativo.Veiculo.Queries.GetAllVeiculos;

public class GetAllVeiculosQueryHandlerTests
{
    private readonly Mock<IVeiculoGateway> _mockGateway = new();

    [Fact]
    public async Task Handle_ReturnsAllVeiculos()
    {
        var veiculos = new List<VeiculoAggregateRoot>
        {
            new() { Id = 1, Placa = "ABC-1D23", IdCliente = 1, Modelo = "Gol", Marca = "Volkswagen" },
            new() { Id = 2, Placa = "DEF-2G34", IdCliente = 2, Modelo = "Polo", Marca = "Volkswagen" }
        };

        _mockGateway.Setup(r => r.GetAllAsync()).ReturnsAsync(veiculos);

        var handler = new GetAllVeiculosQueryHandler(_mockGateway.Object);
        var query = new GetAllVeiculosQuery();

        var result = (await handler.Handle(query, CancellationToken.None)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, v => v.Placa == "ABC-1D23");
        Assert.Contains(result, v => v.Placa == "DEF-2G34");
    }
}
