using Application.UseCases.Administrativo.Veiculo.Queries.GetVeiculoByDono;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Administrativo.Veiculo.Queries.GetVeiculoByDono;

public class GetVeiculosByDonoQueryHandlerTests
{
    private readonly Mock<IVeiculoGateway> _mockGateway = new();

    [Fact]
    public async Task Handle_ReturnsVeiculos_ForDonoId()
    {
        var veiculos = new List<VeiculoAggregateRoot>
        {
            new() { Id = 1, Placa = "ABC-1D23", IdDono = 1, Modelo = "Gol", Marca = "Volkswagen" },
            new() { Id = 2, Placa = "ABD-3F45", IdDono = 1, Modelo = "Fox", Marca = "Volkswagen" }
        };

        _mockGateway.Setup(r => r.GetByDonoIdAsync(1)).ReturnsAsync(veiculos);

        var handler = new GetVeiculosByDonoQueryHandler(_mockGateway.Object);
        var query = new GetVeiculosByDonoQuery { IdDono = 1 };

        var result = (await handler.Handle(query, CancellationToken.None)).ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Equal(1, r.IdDono));
    }
}
