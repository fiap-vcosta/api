using Application.Abstractions.Gateways;
using Application.UseCases.OrdemServico.Commands.RejeitarOrdemServico;
using Application.UseCases.OrdemServico.Commands.RejeitarOrdemServicoPorToken;
using Application.UseCases.OrdemServico.Responses;
using Domain.Exceptions;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.ValueObjects;
using MediatR;
using Moq;

namespace UnitTests.Application.UseCases.OrdemServico.Commands.RejeitarOrdemServicoPorToken;

public class RejeitarOrdemServicoPorTokenCommandHandlerTests
{
    [Fact]
    public async Task Handle_SendsRejeitarCommand_WhenTokenExists()
    {
        // Arrange
        var gateway = new Mock<IOrdemServicoGateway>();
        var mediator = new Mock<IMediator>();
        var ordem = OrdemServicoAggregateRoot.Criar(
            new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" },
            new VeiculoOrdemServico { Placa = "ABC-1234", Marca = "VW", Modelo = "Gol" });
        typeof(OrdemServicoAggregateRoot).GetProperty(nameof(OrdemServicoAggregateRoot.Id))!.SetValue(ordem, 9);

        gateway.Setup(g => g.GetByTokenAsync(ordem.TokenAprovacao)).ReturnsAsync(ordem);
        mediator
            .Setup(m => m.Send(It.IsAny<RejeitarOrdemServicoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RejeitarOrdemServicoCommandResponse
            {
                Id = 9,
                Status = StatusOrdemServico.EmDiagnostico,
                ValorTotal = 0m,
                RecebidaEm = DateTime.UtcNow,
                Cliente = new ClienteOrdemServicoResponse
                {
                    Id = 1, Nome = "Maria", Email = "maria@teste.com"
                },
                Veiculo = new VeiculoOrdemServicoResponse
                {
                    Placa = "ABC-1234", Marca = "VW", Modelo = "Gol"
                },
                Servicos = []
            });

        var handler = new RejeitarOrdemServicoPorTokenCommandHandler(gateway.Object, mediator.Object);

        // Act
        var result = await handler.Handle(
            new RejeitarOrdemServicoPorTokenCommand { TokenAprovacao = ordem.TokenAprovacao },
            CancellationToken.None);

        // Assert
        Assert.Equal(9, result.Id);
        mediator.Verify(
            m => m.Send(It.Is<RejeitarOrdemServicoCommand>(c => c.IdOrdemServico == 9), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_WhenTokenNotFound()
    {
        // Arrange
        var gateway = new Mock<IOrdemServicoGateway>();
        gateway.Setup(g => g.GetByTokenAsync("invalid")).ReturnsAsync((OrdemServicoAggregateRoot?)null);
        var handler = new RejeitarOrdemServicoPorTokenCommandHandler(gateway.Object, new Mock<IMediator>().Object);

        // Act & Assert
        await Assert.ThrowsAsync<DomainNotFoundException>(() =>
            handler.Handle(new RejeitarOrdemServicoPorTokenCommand { TokenAprovacao = "invalid" }, CancellationToken.None));
    }
}
