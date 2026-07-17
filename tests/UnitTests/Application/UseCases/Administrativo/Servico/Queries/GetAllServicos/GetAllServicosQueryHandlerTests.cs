using Application.UseCases.Administrativo.Servico.Queries.GetAllServicos;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Administrativo.Servico.Queries.GetAllServicos;

public class GetAllServicosQueryHandlerTests
{
    private readonly Mock<IServicoGateway> _mockGateway = new();

    [Fact]
    public async Task Handle_ReturnsAllServicos()
    {
        var servicos = new List<ServicoAggregateRoot>
        {
            new() { Id = 1, Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true },
            new() { Id = 2, Codigo = "FRE-001", Nome = "Freio", PrecoPadrao = 250.00m, Ativo = true }
        };

        _mockGateway.Setup(r => r.GetAllAsync()).ReturnsAsync(servicos);

        var handler = new GetAllServicosQueryHandler(_mockGateway.Object);
        var query = new GetAllServicosQuery();

        var result = (await handler.Handle(query, CancellationToken.None)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Codigo == "OLE-001");
        Assert.Contains(result, s => s.Codigo == "FRE-001");
    }
}
