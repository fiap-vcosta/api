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
    private const string DocumentoJoaoSilva = "43372251034";
    private const string DocumentoMariaOliveira = "74694481024";

    [Fact]
    public async Task Handle_SendsRejeitarCommand_WhenTokenAndOwnershipMatch()
    {
        // Arrange
        var ordemGateway = new Mock<IOrdemServicoGateway>();
        var mediator = new Mock<IMediator>();
        var ordem = OrdemServicoAggregateRoot.Criar(
            new ClienteOrdemServico { Id = 1, Nome = "João", Email = "joao@teste.com" },
            new VeiculoOrdemServico { Placa = "ABC-1234", Marca = "VW", Modelo = "Gol" });
        typeof(OrdemServicoAggregateRoot).GetProperty(nameof(OrdemServicoAggregateRoot.Id))!.SetValue(ordem, 9);

        ordemGateway
            .Setup(g => g.GetByTokenEDocumentoAsync(ordem.TokenAprovacao, DocumentoJoaoSilva))
            .ReturnsAsync(ordem);
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
                    Id = 1, Nome = "João", Email = "joao@teste.com"
                },
                Veiculo = new VeiculoOrdemServicoResponse
                {
                    Placa = "ABC-1234", Marca = "VW", Modelo = "Gol"
                },
                Servicos = []
            });

        var handler = new RejeitarOrdemServicoPorTokenCommandHandler(ordemGateway.Object, mediator.Object);

        // Act
        var result = await handler.Handle(
            new RejeitarOrdemServicoPorTokenCommand
            {
                TokenAprovacao = ordem.TokenAprovacao,
                DocumentoCliente = DocumentoJoaoSilva
            },
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
        var ordemGateway = new Mock<IOrdemServicoGateway>();
        ordemGateway
            .Setup(g => g.GetByTokenEDocumentoAsync("invalid", DocumentoJoaoSilva))
            .ReturnsAsync((OrdemServicoAggregateRoot?)null);
        var handler = new RejeitarOrdemServicoPorTokenCommandHandler(
            ordemGateway.Object, new Mock<IMediator>().Object);

        // Act & Assert
        await Assert.ThrowsAsync<DomainNotFoundException>(() =>
            handler.Handle(
                new RejeitarOrdemServicoPorTokenCommand
                {
                    TokenAprovacao = "invalid",
                    DocumentoCliente = DocumentoJoaoSilva
                },
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Throws_WhenOwnershipMismatch()
    {
        // Arrange
        var ordemGateway = new Mock<IOrdemServicoGateway>();
        var ordem = OrdemServicoAggregateRoot.Criar(
            new ClienteOrdemServico { Id = 1, Nome = "João", Email = "joao@teste.com" },
            new VeiculoOrdemServico { Placa = "ABC-1234", Marca = "VW", Modelo = "Gol" });
        ordemGateway
            .Setup(g => g.GetByTokenEDocumentoAsync(ordem.TokenAprovacao, DocumentoMariaOliveira))
            .ReturnsAsync((OrdemServicoAggregateRoot?)null);
        var mediator = new Mock<IMediator>();
        var handler = new RejeitarOrdemServicoPorTokenCommandHandler(ordemGateway.Object, mediator.Object);

        // Act & Assert
        await Assert.ThrowsAsync<DomainNotFoundException>(() =>
            handler.Handle(
                new RejeitarOrdemServicoPorTokenCommand
                {
                    TokenAprovacao = ordem.TokenAprovacao,
                    DocumentoCliente = DocumentoMariaOliveira
                },
                CancellationToken.None));
        mediator.Verify(m => m.Send(It.IsAny<RejeitarOrdemServicoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
