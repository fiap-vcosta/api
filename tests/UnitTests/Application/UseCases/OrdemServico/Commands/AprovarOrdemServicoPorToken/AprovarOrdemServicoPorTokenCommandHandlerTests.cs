using Application.Abstractions.Gateways;
using Application.UseCases.OrdemServico.Commands.AprovarOrdemServico;
using Application.UseCases.OrdemServico.Commands.AprovarOrdemServicoPorToken;
using Application.UseCases.OrdemServico.Responses;
using Domain.Exceptions;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.ValueObjects;
using MediatR;
using Moq;

namespace UnitTests.Application.UseCases.OrdemServico.Commands.AprovarOrdemServicoPorToken;

public class AprovarOrdemServicoPorTokenCommandHandlerTests
{
    [Fact]
    public async Task Handle_SendsAprovarCommand_WhenTokenExists()
    {
        // Arrange
        var gateway = new Mock<IOrdemServicoGateway>();
        var mediator = new Mock<IMediator>();
        var ordem = OrdemServicoAggregateRoot.Criar(
            new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" },
            new VeiculoOrdemServico { Placa = "ABC-1234", Marca = "VW", Modelo = "Gol" });
        typeof(OrdemServicoAggregateRoot).GetProperty(nameof(OrdemServicoAggregateRoot.Id))!.SetValue(ordem, 7);

        gateway.Setup(g => g.GetByTokenAsync(ordem.TokenAprovacao)).ReturnsAsync(ordem);
        mediator
            .Setup(m => m.Send(It.IsAny<AprovarOrdemServicoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AprovarOrdemServicoCommandResponse
            {
                Id = 7,
                Status = StatusOrdemServico.ChecandoEstoque,
                ValorTotal = 100m,
                RecebidaEm = DateTime.UtcNow,
                AprovadaEm = DateTime.UtcNow,
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

        var handler = new AprovarOrdemServicoPorTokenCommandHandler(gateway.Object, mediator.Object);

        // Act
        var result = await handler.Handle(
            new AprovarOrdemServicoPorTokenCommand { TokenAprovacao = ordem.TokenAprovacao },
            CancellationToken.None);

        // Assert
        Assert.Equal(7, result.Id);
        mediator.Verify(
            m => m.Send(It.Is<AprovarOrdemServicoCommand>(c => c.IdOrdemServico == 7), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_WhenTokenNotFound()
    {
        // Arrange
        var gateway = new Mock<IOrdemServicoGateway>();
        gateway.Setup(g => g.GetByTokenAsync("invalid")).ReturnsAsync((OrdemServicoAggregateRoot?)null);
        var handler = new AprovarOrdemServicoPorTokenCommandHandler(gateway.Object, new Mock<IMediator>().Object);

        // Act & Assert
        await Assert.ThrowsAsync<DomainNotFoundException>(() =>
            handler.Handle(new AprovarOrdemServicoPorTokenCommand { TokenAprovacao = "invalid" }, CancellationToken.None));
    }
}
