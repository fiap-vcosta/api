using Domain.Administrativo.Entities;
using Infrastructure.Database;
using Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Infrastructure.Database.Repositories;

public class VeiculoRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly VeiculoRepository _repository;

    public VeiculoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new VeiculoRepository(_context);

        _context.Clientes.Add(new ClienteAggregateRoot { Id = 1, Nome = "Cliente 1", TipoDocumento = TipoDocumento.Cpf, Documento = "12345678901" });
        _context.Veiculos.AddRange(
            new VeiculoAggregateRoot { Id = 1, IdDono = 1, Placa = "ABC-1234", Modelo = "Civic", Marca = "Honda" },
            new VeiculoAggregateRoot { Id = 2, IdDono = 1, Placa = "DEF-5678", Modelo = "Corolla", Marca = "Toyota" }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsVeiculo_WhenVeiculoExists()
    {
        var veiculo = await _repository.GetByIdAsync(1);

        Assert.NotNull(veiculo);
        Assert.Equal(1, veiculo.Id);
        Assert.Equal("ABC-1234", veiculo.Placa);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenVeiculoDoesNotExist()
    {
        var veiculo = await _repository.GetByIdAsync(999);

        Assert.Null(veiculo);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllVeiculos()
    {
        var veiculos = (await _repository.GetAllAsync()).ToList();

        Assert.Equal(2, veiculos.Count);
        Assert.Contains(veiculos, v => v.Placa == "ABC-1234");
        Assert.Contains(veiculos, v => v.Placa == "DEF-5678");
    }

    [Fact]
    public async Task GetByDonoIdAsync_ReturnsVeiculos_ForDonoId()
    {
        var veiculos = (await _repository.GetByDonoIdAsync(1)).ToList();

        Assert.Equal(2, veiculos.Count);
        Assert.All(veiculos, v => Assert.Equal(1, v.IdDono));
    }

    [Fact]
    public async Task GetByPlacaAsync_ReturnsVeiculo_WhenPlacaExists()
    {
        var veiculo = await _repository.GetByPlacaAsync("DEF-5678");

        Assert.NotNull(veiculo);
        Assert.Equal(2, veiculo.Id);
    }

    [Fact]
    public async Task CreateAsync_AddsVeiculo_ToDatabase()
    {
        var newVeiculo = new VeiculoAggregateRoot { IdDono = 1, Placa = "GHI-9012", Modelo = "Fit", Marca = "Honda" };

        await _repository.CreateAsync(newVeiculo);

        var created = await _repository.GetByPlacaAsync("GHI-9012");
        Assert.NotNull(created);
        Assert.Equal("Fit", created.Modelo);
    }

    [Fact]
    public async Task UpdateAsync_ModifiesVeiculo()
    {
        var veiculo = await _repository.GetByIdAsync(1);
        Assert.NotNull(veiculo);
        veiculo.Modelo = "Civic LX";

        await _repository.UpdateAsync(veiculo);

        var updated = await _repository.GetByIdAsync(1);
        Assert.NotNull(updated);
        Assert.Equal("Civic LX", updated.Modelo);
    }

    [Fact]
    public async Task DeleteAsync_RemovesVeiculo()
    {
        await _repository.DeleteAsync(1);

        var deleted = await _repository.GetByIdAsync(1);
        Assert.Null(deleted);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _context.Dispose();
    }
}
