using Domain.OrdemServico.Entities;
using Domain.OrdemServico.ValueObjects;
using Infrastructure.Database;
using Infrastructure.Database.Gateways;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Infrastructure.Database.Gateways;

public class OrdemServicoGatewayTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly OrdemServicoGateway _gateway;

    public OrdemServicoGatewayTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated(); 
        
        _gateway = new OrdemServicoGateway(_context);
    }

    [Fact]
    public async Task CriarAsync_PersistsOrdemServico_WithNestedServiceAndItems()
    {
        // Arrange
        var cliente = new ClienteOrdemServico { Id = 1, Nome = "Cliente Teste", Email = "teste@teste.com" };
        var veiculo = new VeiculoOrdemServico { Placa = "ABC-1234", Marca = "Honda", Modelo = "Civic" };
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        var ordem = OrdemServicoAggregateRoot.Criar(cliente, veiculo);
        ordem.EnviarParaDiagnostico();
        ordem.AdicionarItemServico("Troca de óleo", 100m, servicoCatalogo,
        [
            new ItemNecessario.CriarItemNecessarioParams(1, 1m,
                new ItemEstoqueOrdemServico { Id = 1, Codigo = "ITM-001", Nome = "Filtro", UnidadeMedida = "Unidade" })
        ]);

        // Act
        await _gateway.CriarAsync(ordem);

        // Assert
        var saved = await _gateway.GetByIdAsync(ordem.Id);
        Assert.NotNull(saved);
        Assert.Equal(StatusOrdemServico.EmDiagnostico, saved.Status);
        Assert.Single(saved.Servicos);
        Assert.Single(saved.Servicos.First().ItensNecessarios);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenOrdemServicoDoesNotExist()
    {
        // Arrange
        var nonExistentId = 999;

        // Act
        var ordem = await _gateway.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(ordem);
    }

    [Fact]
    public async Task GetAguardandoPecaPorItemEstoqueAsync_ReturnsOrders_WhenOrderHasStockGap()
    {
        // Arrange
        var cliente = new ClienteOrdemServico { Id = 1, Nome = "Cliente Teste", Email = "teste@teste.com" };
        var veiculo = new VeiculoOrdemServico { Placa = "ABC-1234", Marca = "Honda", Modelo = "Civic" };
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        var ordem = OrdemServicoAggregateRoot.Criar(cliente, veiculo);
        ordem.EnviarParaDiagnostico();
        ordem.AdicionarItemServico("Troca de óleo", 100m, servicoCatalogo,
            new List<ItemNecessario.CriarItemNecessarioParams>
        {
            new(1, 10m, new ItemEstoqueOrdemServico { Id = 1, Codigo = "ITM-001", Nome = "Filtro", UnidadeMedida = "Unidade" })
        });
        ordem.FinalizarDiagnostico();
        ordem.AprovarServicosSugeridos();
        ordem.ChecarItensNecessarios(new Dictionary<int, decimal> { [1] = 0m });
        await _gateway.CriarAsync(ordem);

        // Act
        var result = (await _gateway.GetAguardandoPecaPorItemEstoqueAsync(1)).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(StatusOrdemServico.AguardandoPeca, result.Single().Status);
    }

    [Fact]
    public async Task GetAguardandoPecaPorItemEstoqueAsync_ReturnsEmpty_WhenNoMatchExists()
    {
        // Arrange
        var itemId = 1;

        // Act
        var result = (await _gateway.GetAguardandoPecaPorItemEstoqueAsync(itemId)).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateAsync_PersistsStatusChange()
    {
        // Arrange
        var ordem = OrdemServicoAggregateRoot.Criar(
            new ClienteOrdemServico { Id = 1, Nome = "Cliente", Email = "c@t.com" },
            new VeiculoOrdemServico { Placa = "ABC-1234", Marca = "VW", Modelo = "Gol" });
        await _gateway.CriarAsync(ordem);
        ordem.EnviarParaDiagnostico();

        // Act
        await _gateway.UpdateAsync(ordem);

        // Assert
        var saved = await _gateway.GetByIdAsync(ordem.Id);
        Assert.NotNull(saved);
        Assert.Equal(StatusOrdemServico.EmDiagnostico, saved.Status);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _context.Dispose();
    }
}
