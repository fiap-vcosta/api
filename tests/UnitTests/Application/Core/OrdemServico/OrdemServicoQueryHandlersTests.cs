using Application.Core.OrdemServico.Queries.GetOrdemServicoById;
using Application.Core.OrdemServico.Queries.GetTempoMedioAllServicos;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Repositories;
using Domain.OrdemServico.ValueObjects;
using Moq;

namespace UnitTests.Application.Core.OrdemServico;

public class GetOrdemServicoByIdQueryHandlerTests
{
    private readonly Mock<IOrdemServicoRepository> _mockOrdemServicoRepository;
    private readonly GetOrdemServicoByIdQueryHandler _handler;

    public GetOrdemServicoByIdQueryHandlerTests()
    {
        _mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        _handler = new GetOrdemServicoByIdQueryHandler(_mockOrdemServicoRepository.Object);
    }

    [Fact]
    public async Task Handle_ReturnsOrdemServico_WhenExists()
    {
        // Arrange
        var cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" };
        var veiculo = new VeiculoOrdemServico { Placa = "XYZ-9876", Marca = "Toyota", Modelo = "Corolla" };
        var ordemServico = OrdemServicoAggregateRoot.Criar(cliente, veiculo);

        var query = new GetOrdemServicoByIdQuery { Id = 1 };

        _mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ordemServico);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cliente.Nome, result.Cliente.Nome);
        _mockOrdemServicoRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenOrdemServicoNotFound()
    {
        // Arrange
        var query = new GetOrdemServicoByIdQuery { Id = 999 };
        _mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((OrdemServicoAggregateRoot?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _mockOrdemServicoRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsOrdemServicoWithServices_WhenHasServices()
    {
        // Arrange
        var cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" };
        var veiculo = new VeiculoOrdemServico { Placa = "XYZ-9876", Marca = "Toyota", Modelo = "Corolla" };
        var ordemServico = OrdemServicoAggregateRoot.Criar(cliente, veiculo);
        ordemServico.EnviarParaDiagnostico();
        
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        ordemServico.AdicionarItemServico("Troca de pneus", 200m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>());

        var query = new GetOrdemServicoByIdQuery { Id = 1 };

        _mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ordemServico);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Servicos);
        _mockOrdemServicoRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }
}

public class GetTempoMedioExecucaoAllServicosQueryHandlerTests
{
    private readonly Mock<IItemServicoRepository> _mockItemServicoRepository;
    private readonly GetTempoMedioExecucaoAllServicosQueryHandler _handler;

    public GetTempoMedioExecucaoAllServicosQueryHandlerTests()
    {
        _mockItemServicoRepository = new Mock<IItemServicoRepository>();
        _handler = new GetTempoMedioExecucaoAllServicosQueryHandler(_mockItemServicoRepository.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAverageExecutionTimes_WhenServicesExist()
    {
        // Arrange
        var temposMedios = new List<IItemServicoRepository.TempoMedioExecucaoServico>
        {
            new(1, 5, TimeSpan.FromHours(2)),
            new(2, 3, TimeSpan.FromHours(1)),
            new(3, 8, TimeSpan.FromHours(3))
        };

        var query = new GetTempoMedioExecucaoAllServicosQuery();

        _mockItemServicoRepository
            .Setup(r => r.GetAllTempoMediaExecucaoAsync())
            .ReturnsAsync(temposMedios);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, t => t.idServico == 1 && t.execucaoMedia == TimeSpan.FromHours(2));
        _mockItemServicoRepository.Verify(r => r.GetAllTempoMediaExecucaoAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoServicesExist()
    {
        // Arrange
        var query = new GetTempoMedioExecucaoAllServicosQuery();

        _mockItemServicoRepository
            .Setup(r => r.GetAllTempoMediaExecucaoAsync())
            .ReturnsAsync(new List<IItemServicoRepository.TempoMedioExecucaoServico>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockItemServicoRepository.Verify(r => r.GetAllTempoMediaExecucaoAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsDifferentAverageExecutionTimes()
    {
        // Arrange
        var temposMedios = new List<IItemServicoRepository.TempoMedioExecucaoServico>
        {
            new(1, 10, TimeSpan.FromMinutes(30)),
            new(2, 2, TimeSpan.FromHours(8))
        };

        var query = new GetTempoMedioExecucaoAllServicosQuery();

        _mockItemServicoRepository
            .Setup(r => r.GetAllTempoMediaExecucaoAsync())
            .ReturnsAsync(temposMedios);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        var serviceRapido = result.First(t => t.idServico == 1);
        var servicoLongo = result.First(t => t.idServico == 2);
        Assert.Equal(TimeSpan.FromMinutes(30), serviceRapido.execucaoMedia);
        Assert.Equal(TimeSpan.FromHours(8), servicoLongo.execucaoMedia);
    }
}
