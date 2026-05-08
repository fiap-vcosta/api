using Application.Administrativo.Veiculo.Queries;
using Application.Administrativo.Veiculo.Queries.GetAllVeiculos;
using Application.Administrativo.Veiculo.Queries.GetVeiculoByDono;
using Application.Administrativo.Veiculo.Queries.GetVeiculoById;
using Domain.Administrativo.Repositories;
using Moq;

namespace UnitTests.Application.Administrativo.Veiculo;

public class VeiculoQueryHandlersTests
{
    private readonly Mock<IVeiculoRepository> _mockRepository = new();

    [Fact]
    public async Task GetVeiculoByIdQueryHandler_ReturnsVeiculo_WhenVeiculoExists()
    {
        var veiculo = new Domain.Administrativo.Entities.VeiculoAggregateRoot { Id = 1, Placa = "ABC-1D23", DonoId = 1, Modelo = "Gol", Marca = "Volkswagen" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(veiculo);

        var handler = new GetVeiculoByIdQueryHandler(_mockRepository.Object);
        var query = new GetVeiculoByIdQuery { Id = 1 };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("ABC-1D23", result.Placa);
    }

    [Fact]
    public async Task GetVeiculoByIdQueryHandler_ReturnsNull_WhenVeiculoDoesNotExist()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Domain.Administrativo.Entities.VeiculoAggregateRoot?)null);

        var handler = new GetVeiculoByIdQueryHandler(_mockRepository.Object);
        var query = new GetVeiculoByIdQuery { Id = 999 };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllVeiculosQueryHandler_ReturnsAllVeiculos()
    {
        var veiculos = new List<Domain.Administrativo.Entities.VeiculoAggregateRoot>
        {
            new() { Id = 1, Placa = "ABC-1D23", DonoId = 1, Modelo = "Gol", Marca = "Volkswagen" },
            new() { Id = 2, Placa = "DEF-2G34", DonoId = 2, Modelo = "Polo", Marca = "Volkswagen" }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(veiculos);

        var handler = new GetAllVeiculosQueryHandler(_mockRepository.Object);
        var query = new GetAllVeiculosQuery();

        var result = (await handler.Handle(query, CancellationToken.None)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, v => v.Placa == "ABC-1D23");
        Assert.Contains(result, v => v.Placa == "DEF-2G34");
    }

    [Fact]
    public async Task GetVeiculosByDonoQueryHandler_ReturnsVeiculos_ForDonoId()
    {
        var veiculos = new List<Domain.Administrativo.Entities.VeiculoAggregateRoot>
        {
            new() { Id = 1, Placa = "ABC-1D23", DonoId = 1, Modelo = "Gol", Marca = "Volkswagen" },
            new() { Id = 2, Placa = "ABD-3F45", DonoId = 1, Modelo = "Fox", Marca = "Volkswagen" }
        };

        _mockRepository.Setup(r => r.GetByDonoIdAsync(1)).ReturnsAsync(veiculos);

        var handler = new GetVeiculosByDonoQueryHandler(_mockRepository.Object);
        var query = new GetVeiculosByDonoQuery { DonoId = 1 };

        var result = (await handler.Handle(query, CancellationToken.None)).ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Equal(1, r.DonoId));
    }
}
