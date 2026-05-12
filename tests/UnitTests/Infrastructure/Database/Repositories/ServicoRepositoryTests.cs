using Domain.Administrativo.Entities;
using Infrastructure.Database;
using Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Infrastructure.Database.Repositories;

public class ServicoRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ServicoRepository _repository;

    public ServicoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new ServicoRepository(_context);

        _context.Servicos.AddRange(
            new ServicoAggregateRoot { Id = 1, Codigo = "SRV-01", Nome = "Troca de óleo", PrecoPadrao = 120.00m, Ativo = true },
            new ServicoAggregateRoot { Id = 2, Codigo = "SRV-02", Nome = "Alinhamento", PrecoPadrao = 85.50m, Ativo = true }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsServico_WhenServicoExists()
    {
        var servico = await _repository.GetByIdAsync(1);

        Assert.NotNull(servico);
        Assert.Equal("SRV-01", servico.Codigo);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenServicoDoesNotExist()
    {
        var servico = await _repository.GetByIdAsync(999);

        Assert.Null(servico);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllServicos()
    {
        var servicos = (await _repository.GetAllAsync()).ToList();

        Assert.Equal(2, servicos.Count);
        Assert.Contains(servicos, s => s.Codigo == "SRV-01");
        Assert.Contains(servicos, s => s.Codigo == "SRV-02");
    }

    [Fact]
    public async Task GetByCodigoAsync_ReturnsServico_WhenCodigoExists()
    {
        var servico = await _repository.GetByCodigoAsync("SRV-02");

        Assert.NotNull(servico);
        Assert.Equal("Alinhamento", servico.Nome);
    }

    [Fact]
    public async Task CreateAsync_AddsServico_ToDatabase()
    {
        var newServico = new ServicoAggregateRoot { Codigo = "SRV-03", Nome = "Balanceamento", PrecoPadrao = 75.00m, Ativo = true };

        await _repository.CreateAsync(newServico);

        var created = await _repository.GetByCodigoAsync("SRV-03");
        Assert.NotNull(created);
        Assert.Equal("Balanceamento", created.Nome);
    }

    [Fact]
    public async Task UpdateAsync_ModifiesServico()
    {
        var servico = await _repository.GetByIdAsync(1);
        Assert.NotNull(servico);
        servico.Nome = "Troca de óleo premium";

        await _repository.UpdateAsync(servico);

        var updated = await _repository.GetByIdAsync(1);
        Assert.NotNull(updated);
        Assert.Equal("Troca de óleo premium", updated.Nome);
    }

    [Fact]
    public async Task DeleteAsync_RemovesServico()
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
