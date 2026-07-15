using Application.Abstractions.Services;
using Application.Estoque.ItemEstoque.Commands.ConfirmarUtilizacaoItensEstoque;
using Application.Estoque.ItemEstoque.Commands.EnviarNotificacaoParaCompra;
using Application.Estoque.ItemEstoque.Commands.RegistrarEntradaEstoque;
using Application.Estoque.ItemEstoque.Commands.TravarItensNecessarios;
using Domain.Administrativo.Entities;
using Domain.Estoque.Entities;
using Domain.Estoque.Events;
using Domain.Estoque.Repositories;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Repositories;
using Domain.OrdemServico.ValueObjects;
using MediatR;
using Moq;

namespace UnitTests.Application.Estoque.ItemEstoque;

public class TravarItensNecessariosCommandHandlerTests
{
    [Fact]
    public async Task Handle_LocksStock_WhenItemExists()
    {
        // Arrange
        var mockRepository = new Mock<IItemEstoqueRepository>();
        var item = new ItemEstoqueAggregateRoot { Id = 1, Nome = "Pneu", Saldo = 10m, SaldoReservado = 0m };

        mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
        mockRepository.Setup(r => r.UpdateAsync(It.IsAny<ItemEstoqueAggregateRoot>())).Returns(Task.CompletedTask);

        var handler = new TravarItensNecessariosCommandHandler(mockRepository.Object);

        // Act
        await handler.Handle(new TravarItensNecessariosCommand { IdItemEstoque = 1, QuantidadeNecessaria = 4m }, CancellationToken.None);

        // Assert
        Assert.Equal(4m, item.SaldoReservado);
        mockRepository.Verify(r => r.UpdateAsync(item), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_WhenItemNotFound()
    {
        // Arrange
        var mockRepository = new Mock<IItemEstoqueRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ItemEstoqueAggregateRoot?)null);

        var handler = new TravarItensNecessariosCommandHandler(mockRepository.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new TravarItensNecessariosCommand { IdItemEstoque = 999, QuantidadeNecessaria = 1m }, CancellationToken.None));
    }
}

public class RegistrarEntradaEstoqueCommandHandlerTests
{
    [Fact]
    public async Task Handle_RegistersEntryAndPublishesEvent_WhenItemExists()
    {
        // Arrange
        var mockRepository = new Mock<IItemEstoqueRepository>();
        var mockMediator = new Mock<IMediator>();
        var item = new ItemEstoqueAggregateRoot { Id = 1, Nome = "Filtro", Saldo = 10m };

        mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
        mockRepository.Setup(r => r.UpdateAsync(It.IsAny<ItemEstoqueAggregateRoot>())).Returns(Task.CompletedTask);
        mockMediator
            .Setup(m => m.Publish(It.IsAny<ChegadaDeItensRegistradaEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new RegistrarEntradaEstoqueCommandHandler(mockRepository.Object, mockMediator.Object);

        // Act
        var result = await handler.Handle(
            new RegistrarEntradaEstoqueCommand { IdItemEstoque = 1, QuantidadeRecebida = 50m },
            CancellationToken.None);

        // Assert
        Assert.Equal(60m, result.Saldo);
        mockMediator.Verify(
            m => m.Publish(It.Is<ChegadaDeItensRegistradaEvent>(e => e.ItemEstoqueAggregateRoot.Id == 1), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_WhenItemNotFound()
    {
        // Arrange
        var mockRepository = new Mock<IItemEstoqueRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ItemEstoqueAggregateRoot?)null);

        var handler = new RegistrarEntradaEstoqueCommandHandler(mockRepository.Object, new Mock<IMediator>().Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new RegistrarEntradaEstoqueCommand { IdItemEstoque = 999, QuantidadeRecebida = 1m }, CancellationToken.None));
    }
}

public class ConfirmarUtilizacaoItensEstoqueCommandHandlerTests
{
    [Fact]
    public async Task Handle_ConfirmsUtilization_WhenOrdemExists()
    {
        // Arrange
        var mockOrdemRepository = new Mock<IOrdemServicoRepository>();
        var mockItemRepository = new Mock<IItemEstoqueRepository>();

        var ordem = CriarOrdemComItensTravados();
        var item = new ItemEstoqueAggregateRoot { Id = 100, Nome = "Pneu", Saldo = 10m, SaldoReservado = 4m };

        mockOrdemRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ordem);
        mockItemRepository.Setup(r => r.GetUtilizadosByOrdemServico(1)).ReturnsAsync(new List<ItemEstoqueAggregateRoot> { item });
        mockItemRepository.Setup(r => r.UpdateAsync(It.IsAny<ItemEstoqueAggregateRoot>())).Returns(Task.CompletedTask);

        var handler = new ConfirmarUtilizacaoItensEstoqueCommandHandler(mockOrdemRepository.Object, mockItemRepository.Object);

        // Act
        await handler.Handle(new ConfirmarUtilizacaoItensEstoqueCommand { IdOrdemServico = 1 }, CancellationToken.None);

        // Assert
        Assert.Equal(6m, item.Saldo);
        Assert.Equal(0m, item.SaldoReservado);
        mockItemRepository.Verify(r => r.UpdateAsync(item), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_WhenOrdemNotFound()
    {
        // Arrange
        var mockOrdemRepository = new Mock<IOrdemServicoRepository>();
        mockOrdemRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((OrdemServicoAggregateRoot?)null);

        var handler = new ConfirmarUtilizacaoItensEstoqueCommandHandler(
            mockOrdemRepository.Object,
            new Mock<IItemEstoqueRepository>().Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new ConfirmarUtilizacaoItensEstoqueCommand { IdOrdemServico = 999 }, CancellationToken.None));
    }

    private static OrdemServicoAggregateRoot CriarOrdemComItensTravados()
    {
        var ordem = OrdemServicoAggregateRoot.Criar(
            new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" },
            new VeiculoOrdemServico { Placa = "XYZ-9876", Marca = "Toyota", Modelo = "Corolla" });
        typeof(OrdemServicoAggregateRoot).GetProperty(nameof(OrdemServicoAggregateRoot.Id))!.SetValue(ordem, 1);

        var catalogo = new ServicoCatalogo { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        ordem.EnviarParaDiagnostico();
        ordem.AdicionarItemServico("Troca", 200m, catalogo,
        [
            new ItemNecessario.CriarItemNecessarioParams(1, 4m, new ItemEstoqueOrdemServico { Id = 100, Nome = "Pneu" })
        ]);
        ordem.FinalizarDiagnostico();
        ordem.AprovarServicosSugeridos();
        ordem.ChecarItensNecessarios(new Dictionary<int, decimal> { [100] = 10m });
        ordem.TravarItensNecessarios();
        return ordem;
    }
}

public class EnviarNotificacaoParaCompraCommandHandlerTests
{
    [Fact]
    public async Task Handle_NotifiesAtendente()
    {
        // Arrange
        var mockNotificacao = new Mock<INotificacaoService>();
        mockNotificacao
            .Setup(n => n.NotificarUsuariosPorTipo(TipoUsuario.Atendente, It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var handler = new EnviarNotificacaoParaCompraCommandHandle(mockNotificacao.Object);
        var command = new EnviarNotificacaoParaCompraCommand
        {
            IdItemEstoque = 1,
            IdOrdemServico = 10,
            NomeItem = "Pneu",
            QuantidadeFaltando = 5m
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockNotificacao.Verify(
            n => n.NotificarUsuariosPorTipo(
                TipoUsuario.Atendente,
                It.Is<string>(s => s.Contains("5") && s.Contains("Pneu") && s.Contains("10"))),
            Times.Once);
    }
}
