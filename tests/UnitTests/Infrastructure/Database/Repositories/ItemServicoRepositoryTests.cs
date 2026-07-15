using Domain.OrdemServico.Entities;
using Domain.OrdemServico.ValueObjects;
using Infrastructure.Database;
using Infrastructure.Database.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Infrastructure.Database.Repositories;

public class ItemServicoRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ItemServicoRepository _repository;
    private readonly OrdemServicoRepository _ordemRepository;

    public ItemServicoRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
        _repository = new ItemServicoRepository(_context);
        _ordemRepository = new OrdemServicoRepository(_context);
    }

    [Fact]
    public async Task GetAllTempoMediaExecucaoAsync_ReturnsAverages_WhenServicesHaveExecutionTimes()
    {
        // Arrange
        var ordem = OrdemServicoAggregateRoot.Criar(
            new ClienteOrdemServico { Id = 1, Nome = "Cliente", Email = "c@t.com" },
            new VeiculoOrdemServico { Placa = "ABC-1234", Marca = "VW", Modelo = "Gol" });
        ordem.EnviarParaDiagnostico();
        var catalogo = new ServicoCatalogo { Id = 10, Nome = "Troca", Codigo = "TRC-001" };
        ordem.AdicionarItemServico("Troca", 100m, catalogo, []);
        ordem.FinalizarDiagnostico();
        ordem.AprovarServicosSugeridos();
        ordem.ChecarItensNecessarios(new Dictionary<int, decimal>());
        var servicoId = ordem.Servicos.First().Id;
        ordem.ConfirmarExecucao(
        [
            new ServicoExecutado
            {
                IdServico = servicoId,
                IniciadoEm = DateTime.UtcNow.AddHours(-2),
                FinalizadoEm = DateTime.UtcNow
            }
        ]);
        await _ordemRepository.CriarAsync(ordem);

        // Act
        var tempos = await _repository.GetAllTempoMediaExecucaoAsync();

        // Assert
        Assert.Contains(tempos, t => t.idServico == 10 && t.totalExecucoes == 1 && t.execucaoMedia > TimeSpan.Zero);
    }

    [Fact]
    public async Task GetAllTempoMediaExecucaoAsync_ReturnsEmpty_WhenNoCompletedServices()
    {
        // Arrange / Act
        var tempos = await _repository.GetAllTempoMediaExecucaoAsync();

        // Assert
        Assert.Empty(tempos);
    }

    public void Dispose() => _context.Dispose();
}
