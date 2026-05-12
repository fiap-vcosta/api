using Domain.Estoque.Entities;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.ValueObjects;
using Infrastructure.Database;
using Infrastructure.Database.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Infrastructure.Database.Repositories;

public class ItemEstoqueRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ItemEstoqueRepository _repository;

    public ItemEstoqueRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated(); 
        
        _repository = new ItemEstoqueRepository(_context);

        _context.ItensEstoque.AddRange(
            new ItemEstoqueAggregateRoot { Id = 1, Codigo = "ITM-001", Tipo = ItemTipo.Peca, Nome = "Filtro de óleo", UnidadeMedida = UnidadeMedida.Unidade, PrecoVenda = 45.5m, Saldo = 10m, SaldoReservado = 2m },
            new ItemEstoqueAggregateRoot { Id = 2, Codigo = "ITM-002", Tipo = ItemTipo.Insumo, Nome = "Óleo", UnidadeMedida = UnidadeMedida.Litro, PrecoVenda = 25.0m, Saldo = 20m, SaldoReservado = 3m }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsItemEstoque_WhenItemExists()
    {
        // Arrange
        var existingId = 1;

        // Act
        var item = await _repository.GetByIdAsync(existingId);

        // Assert
        Assert.NotNull(item);
        Assert.Equal("ITM-001", item.Codigo);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenItemDoesNotExist()
    {
        // Arrange
        var nonExistentId = 999;

        // Act
        var item = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(item);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllItemsEstoque()
    {
        // Arrange

        // Act
        var items = (await _repository.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, items.Count);
    }

    [Fact]
    public async Task GetByCodigoAsync_ReturnsItemEstoque_WhenCodigoExists()
    {
        // Arrange
        var existingCodigo = "ITM-002";

        // Act
        var item = await _repository.GetByCodigoAsync(existingCodigo);

        // Assert
        Assert.NotNull(item);
        Assert.Equal(ItemTipo.Insumo, item.Tipo);
    }

    [Fact]
    public async Task GetEBloquearItensAsync_ReturnsEmptyList_WhenIdsAreEmpty()
    {
        // Arrange
        var emptyIds = new List<int>();

        // Act
        var itens = (await _repository.GetEBloquearItensAsync(emptyIds)).ToList();

        // Assert
        Assert.Empty(itens);
    }

    [Fact]
    public async Task CreateAsync_AddsItemEstoque_ToDatabase()
    {
        // Arrange
        var newItem = new ItemEstoqueAggregateRoot { Codigo = "ITM-003", Tipo = ItemTipo.Peca, Nome = "Bateria", UnidadeMedida = UnidadeMedida.Unidade, PrecoVenda = 150m, Saldo = 5m, SaldoReservado = 0m };

        // Act
        await _repository.CreateAsync(newItem);

        // Assert
        var created = await _repository.GetByCodigoAsync("ITM-003");
        Assert.NotNull(created);
        Assert.Equal("Bateria", created.Nome);
    }

    [Fact]
    public async Task UpdateAsync_ModifiesItemEstoque()
    {
        // Arrange
        var item = await _repository.GetByIdAsync(1);
        Assert.NotNull(item);
        item.Nome = "Filtro de óleo premium";

        // Act
        await _repository.UpdateAsync(item);

        // Assert
        var updated = await _repository.GetByIdAsync(1);
        Assert.NotNull(updated);
        Assert.Equal("Filtro de óleo premium", updated.Nome);
    }

    [Fact]
    public async Task DeleteAsync_RemovesItemEstoque()
    {
        // Arrange
        var idToDelete = 1;

        // Act
        await _repository.DeleteAsync(idToDelete);

        // Assert
        var deleted = await _repository.GetByIdAsync(1);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task GetUtilizadosByOrdemServico_ReturnsEstoqueItems_WhenOrdemServicoContainsItemEstoque()
    {
        // Arrange
        var cliente = new ClienteOrdemServico { Id = 1, Nome = "Cliente Teste", Email = "teste@teste.com" };
        var veiculo = new VeiculoOrdemServico { Placa = "ABC-1234", Marca = "Honda", Modelo = "Civic" };
        var ordem = OrdemServicoAggregateRoot.Criar(cliente, veiculo);
        ordem.EnviarParaDiagnostico();
        ordem.AdicionarItemServico("Troca de óleo", 100m, new List<ItemNecessario.CriarItemNecessarioParams>
        {
            new(1, 1m, new ItemEstoqueOrdemServico { Id = 1, Codigo = "ITM-001", Nome = "Filtro de óleo", UnidadeMedida = "Unidade" })
        });
        _context.OrdensServico.Add(ordem);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetUtilizadosByOrdemServico(ordem.Id)).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("ITM-001", result[0].Codigo);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _context.Dispose();
    }
}
