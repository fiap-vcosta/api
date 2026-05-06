using Application.Servico.Queries;
using Application.Servico.Queries.Handlers;
using Domain.Entities;
using Domain.Repositories;
using Moq;

namespace UnitTests.Application.Handlers;

public class ServicoQueryHandlersTests
{
    private readonly Mock<IServicoRepository> _mockRepository = new();

    [Fact]
    public async Task GetServicoByIdQueryHandler_ReturnsServico_WhenServicoExists()
    {
        var servico = new Servico { Id = 1, Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(servico);

        var handler = new GetServicoByIdQueryHandler(_mockRepository.Object);
        var query = new GetServicoByIdQuery { Id = 1 };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("OLE-001", result.Codigo);
    }

    [Fact]
    public async Task GetServicoByIdQueryHandler_ReturnsNull_WhenServicoDoesNotExist()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Servico?)null);

        var handler = new GetServicoByIdQueryHandler(_mockRepository.Object);
        var query = new GetServicoByIdQuery { Id = 999 };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllServicosQueryHandler_ReturnsAllServicos()
    {
        var servicos = new List<Servico>
        {
            new() { Id = 1, Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true },
            new() { Id = 2, Codigo = "FRE-001", Nome = "Freio", PrecoPadrao = 250.00m, Ativo = true }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(servicos);

        var handler = new GetAllServicosQueryHandler(_mockRepository.Object);
        var query = new GetAllServicosQuery();

        var result = (await handler.Handle(query, CancellationToken.None)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Codigo == "OLE-001");
        Assert.Contains(result, s => s.Codigo == "FRE-001");
    }
}
