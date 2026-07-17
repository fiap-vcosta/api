using Application.UseCases.Administrativo.Servico.Queries.GetServicoById;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Administrativo.Servico.Queries.GetServicoById;

public class GetServicoByIdQueryHandlerTests
{
    private readonly Mock<IServicoGateway> _mockGateway = new();

    [Fact]
    public async Task Handle_ReturnsServico_WhenServicoExists()
    {
        var servico = new ServicoAggregateRoot { Id = 1, Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true };
        _mockGateway.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(servico);

        var handler = new GetServicoByIdQueryHandler(_mockGateway.Object);
        var query = new GetServicoByIdQuery { Id = 1 };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("OLE-001", result.Codigo);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenServicoDoesNotExist()
    {
        _mockGateway.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ServicoAggregateRoot?)null);

        var handler = new GetServicoByIdQueryHandler(_mockGateway.Object);
        var query = new GetServicoByIdQuery { Id = 999 };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }
}
