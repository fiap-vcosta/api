using Application.Core.OrdemServico.Commands.AlocarEstoqueOrdemServico;
using Application.Estoque.ItemEstoque.Commands.EnviarNotificacaoParaCompra;
using Application.Estoque.ItemEstoque.Commands.TravarItensNecessarios;
using Domain.Estoque.Entities;
using Domain.Estoque.Repositories;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Repositories;
using Domain.OrdemServico.ValueObjects;
using MediatR;
using Moq;

namespace UnitTests.Application.Core.OrdemServico;

public class AlocarEstoqueOrdemServicoCommandHandlerTests
{
    [Fact]
    public async Task Handle_LocksStock_WhenStockIsSufficient()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        var mockItemEstoqueRepository = new Mock<IItemEstoqueRepository>();
        var mockMediator = new Mock<IMediator>();

        var ordemServico = CriarOrdemEmChecandoEstoque(quantidadeNecessaria: 4m);
        ReflectSetId(ordemServico, 1);

        var itemEstoque = new ItemEstoqueAggregateRoot
        {
            Id = 100,
            Codigo = "PNU-001",
            Nome = "Pneu",
            Saldo = 10m,
            SaldoReservado = 0m
        };

        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ordemServico);
        mockItemEstoqueRepository
            .Setup(r => r.GetEBloquearItensAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<ItemEstoqueAggregateRoot> { itemEstoque });
        mockOrdemServicoRepository.Setup(r => r.UpdateAsync(It.IsAny<OrdemServicoAggregateRoot>())).Returns(Task.CompletedTask);
        mockMediator
            .Setup(m => m.Send(It.IsAny<TravarItensNecessariosCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var handler = new AlocarEstoqueOrdemServicoCommandHandler(
            mockOrdemServicoRepository.Object,
            mockItemEstoqueRepository.Object,
            mockMediator.Object);

        // Act
        await handler.Handle(new AlocarEstoqueOrdemServicoCommand(1), CancellationToken.None);

        // Assert
        Assert.Equal(StatusOrdemServico.LiberadaParaExecucao, ordemServico.Status);
        mockMediator.Verify(
            m => m.Send(It.Is<TravarItensNecessariosCommand>(c => c.IdItemEstoque == 100 && c.QuantidadeNecessaria == 4m), It.IsAny<CancellationToken>()),
            Times.Once);
        mockOrdemServicoRepository.Verify(r => r.UpdateAsync(ordemServico), Times.Once);
    }

    [Fact]
    public async Task Handle_RequestsPurchase_WhenStockIsInsufficient()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        var mockItemEstoqueRepository = new Mock<IItemEstoqueRepository>();
        var mockMediator = new Mock<IMediator>();

        var ordemServico = CriarOrdemEmChecandoEstoque(quantidadeNecessaria: 10m);
        ReflectSetId(ordemServico, 1);

        var itemEstoque = new ItemEstoqueAggregateRoot
        {
            Id = 100,
            Codigo = "PNU-001",
            Nome = "Pneu",
            Saldo = 5m,
            SaldoReservado = 0m
        };

        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ordemServico);
        mockItemEstoqueRepository
            .Setup(r => r.GetEBloquearItensAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<ItemEstoqueAggregateRoot> { itemEstoque });
        mockOrdemServicoRepository.Setup(r => r.UpdateAsync(It.IsAny<OrdemServicoAggregateRoot>())).Returns(Task.CompletedTask);
        mockMediator
            .Setup(m => m.Send(It.IsAny<EnviarNotificacaoParaCompraCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var handler = new AlocarEstoqueOrdemServicoCommandHandler(
            mockOrdemServicoRepository.Object,
            mockItemEstoqueRepository.Object,
            mockMediator.Object);

        // Act
        await handler.Handle(new AlocarEstoqueOrdemServicoCommand(1), CancellationToken.None);

        // Assert
        Assert.Equal(StatusOrdemServico.AguardandoPeca, ordemServico.Status);
        mockMediator.Verify(
            m => m.Send(
                It.Is<EnviarNotificacaoParaCompraCommand>(c =>
                    c.IdItemEstoque == 100 &&
                    c.IdOrdemServico == 1 &&
                    c.QuantidadeFaltando == 5m),
                It.IsAny<CancellationToken>()),
            Times.Once);
        mockOrdemServicoRepository.Verify(r => r.UpdateAsync(ordemServico), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsKeyNotFoundException_WhenOrdemServicoNotFound()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        var mockItemEstoqueRepository = new Mock<IItemEstoqueRepository>();
        var mockMediator = new Mock<IMediator>();

        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((OrdemServicoAggregateRoot?)null);

        var handler = new AlocarEstoqueOrdemServicoCommandHandler(
            mockOrdemServicoRepository.Object,
            mockItemEstoqueRepository.Object,
            mockMediator.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new AlocarEstoqueOrdemServicoCommand(999), CancellationToken.None));
    }

    private static OrdemServicoAggregateRoot CriarOrdemEmChecandoEstoque(decimal quantidadeNecessaria)
    {
        var cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" };
        var veiculo = new VeiculoOrdemServico { Placa = "XYZ-9876", Marca = "Toyota", Modelo = "Corolla" };
        var ordem = OrdemServicoAggregateRoot.Criar(cliente, veiculo);
        ReflectSetId(ordem, 1);

        var servicoCatalogo = new ServicoCatalogo { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        ordem.EnviarParaDiagnostico();
        ordem.AdicionarItemServico(
            "Troca de pneus",
            200m,
            servicoCatalogo,
            new List<ItemNecessario.CriarItemNecessarioParams>
            {
                new(1, quantidadeNecessaria, new ItemEstoqueOrdemServico { Id = 100, Nome = "Pneu" })
            });
        ordem.FinalizarDiagnostico();
        ordem.AprovarServicosSugeridos();

        return ordem;
    }

    private static void ReflectSetId(OrdemServicoAggregateRoot ordem, int id)
    {
        typeof(OrdemServicoAggregateRoot).GetProperty(nameof(OrdemServicoAggregateRoot.Id))!.SetValue(ordem, id);
    }
}
