using Application.Abstractions.Gateways;
using Application.UseCases.OrdemServico.Commands.AprovarOrdemServico;
using Application.UseCases.OrdemServico.Commands.AprovarOrdemServicoPorToken;
using Application.UseCases.OrdemServico.Responses;
using Domain.Administrativo.Entities;
using Domain.Exceptions;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.ValueObjects;
using MediatR;
using Moq;

namespace UnitTests.Application.UseCases.OrdemServico.Commands.AprovarOrdemServicoPorToken;

public class AprovarOrdemServicoPorTokenCommandHandlerTests
{
    private const string DocumentoDono = "43372251034";
    private const string DocumentoOutro = "74694481024";

    [Fact]
    public async Task Handle_SendsAprovarCommand_WhenTokenAndOwnershipMatch()
    {
        // Arrange
        var ordemGateway = new Mock<IOrdemServicoGateway>();
        var clienteGateway = new Mock<IClienteGateway>();
        var mediator = new Mock<IMediator>();
        var ordem = OrdemServicoAggregateRoot.Criar(
            new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" },
            new VeiculoOrdemServico { Placa = "ABC-1234", Marca = "VW", Modelo = "Gol" });
        typeof(OrdemServicoAggregateRoot).GetProperty(nameof(OrdemServicoAggregateRoot.Id))!.SetValue(ordem, 7);

        ordemGateway.Setup(g => g.GetByTokenAsync(ordem.TokenAprovacao)).ReturnsAsync(ordem);
        clienteGateway.Setup(g => g.GetByIdAsync(1)).ReturnsAsync(new ClienteAggregateRoot
        {
            Id = 1,
            Nome = "Maria",
            Documento = DocumentoDono,
            TipoDocumento = TipoDocumento.Cpf
        });
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

        var handler = new AprovarOrdemServicoPorTokenCommandHandler(
            ordemGateway.Object, clienteGateway.Object, mediator.Object);

        // Act
        var result = await handler.Handle(
            new AprovarOrdemServicoPorTokenCommand
            {
                TokenAprovacao = ordem.TokenAprovacao,
                DocumentoCliente = DocumentoDono
            },
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
        var ordemGateway = new Mock<IOrdemServicoGateway>();
        ordemGateway.Setup(g => g.GetByTokenAsync("invalid")).ReturnsAsync((OrdemServicoAggregateRoot?)null);
        var handler = new AprovarOrdemServicoPorTokenCommandHandler(
            ordemGateway.Object, new Mock<IClienteGateway>().Object, new Mock<IMediator>().Object);

        // Act & Assert
        await Assert.ThrowsAsync<DomainNotFoundException>(() =>
            handler.Handle(
                new AprovarOrdemServicoPorTokenCommand
                {
                    TokenAprovacao = "invalid",
                    DocumentoCliente = DocumentoDono
                },
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Throws_WhenOwnershipMismatch()
    {
        // Arrange
        var ordemGateway = new Mock<IOrdemServicoGateway>();
        var clienteGateway = new Mock<IClienteGateway>();
        var ordem = OrdemServicoAggregateRoot.Criar(
            new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" },
            new VeiculoOrdemServico { Placa = "ABC-1234", Marca = "VW", Modelo = "Gol" });
        ordemGateway.Setup(g => g.GetByTokenAsync(ordem.TokenAprovacao)).ReturnsAsync(ordem);
        clienteGateway.Setup(g => g.GetByIdAsync(1)).ReturnsAsync(new ClienteAggregateRoot
        {
            Id = 1,
            Documento = DocumentoDono,
            TipoDocumento = TipoDocumento.Cpf
        });
        var mediator = new Mock<IMediator>();
        var handler = new AprovarOrdemServicoPorTokenCommandHandler(
            ordemGateway.Object, clienteGateway.Object, mediator.Object);

        // Act & Assert
        await Assert.ThrowsAsync<DomainNotFoundException>(() =>
            handler.Handle(
                new AprovarOrdemServicoPorTokenCommand
                {
                    TokenAprovacao = ordem.TokenAprovacao,
                    DocumentoCliente = DocumentoOutro
                },
                CancellationToken.None));
        mediator.Verify(m => m.Send(It.IsAny<AprovarOrdemServicoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
