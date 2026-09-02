using Application.UseCases.Administrativo.Veiculo.Queries.GetVeiculoById;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Administrativo.Veiculo.Queries.GetVeiculoById;

public class GetVeiculoByIdQueryHandlerTests
{
    private readonly Mock<IVeiculoGateway> _mockGateway = new();

    [Fact]
    public async Task Handle_ReturnsVeiculo_WhenVeiculoExists()
    {
        var veiculo = new VeiculoAggregateRoot() { Id = 1, Placa = "ABC-1D23", IdCliente = 1, Modelo = "Gol", Marca = "Volkswagen" };
        _mockGateway.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(veiculo);

        var handler = new GetVeiculoByIdQueryHandler(_mockGateway.Object);
        var query = new GetVeiculoByIdQuery { Id = 1 };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("ABC-1D23", result.Placa);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenVeiculoDoesNotExist()
    {
        _mockGateway.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((VeiculoAggregateRoot?)null);

        var handler = new GetVeiculoByIdQueryHandler(_mockGateway.Object);
        var query = new GetVeiculoByIdQuery { Id = 999 };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }
}
