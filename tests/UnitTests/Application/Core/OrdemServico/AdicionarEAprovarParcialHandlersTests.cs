using Application.Core.OrdemServico.Commands.AdicionarItemOrdemServico;
using Application.Core.OrdemServico.Commands.AprovarServicosParcialmente;
using Domain.Administrativo.Entities;
using Domain.Administrativo.Repositories;
using Domain.Estoque.Entities;
using Domain.Estoque.Repositories;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Repositories;
using Domain.OrdemServico.ValueObjects;
using Moq;

namespace UnitTests.Application.Core.OrdemServico;

public class AdicionarItemOrdemServicoCommandHandlerTests
{
    [Fact]
    public async Task Handle_AddsServiceItem_WhenCommandIsValid()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        var mockServicoRepository = new Mock<IServicoRepository>();
        var mockItemEstoqueRepository = new Mock<IItemEstoqueRepository>();

        var ordem = CriarOrdemEmDiagnostico();
        var servico = new ServicoAggregateRoot { Id = 5, Codigo = "OLE-001", Nome = "Troca de Óleo", PrecoPadrao = 150m, Ativo = true };
        var itemEstoque = new ItemEstoqueAggregateRoot { Id = 10, Codigo = "FLT-001", Nome = "Filtro", UnidadeMedida = UnidadeMedida.Unidade };

        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ordem);
        mockServicoRepository.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(servico);
        mockItemEstoqueRepository.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(itemEstoque);
        mockOrdemServicoRepository.Setup(r => r.UpdateAsync(It.IsAny<OrdemServicoAggregateRoot>())).Returns(Task.CompletedTask);

        var handler = new AdicionarItemOrdemServicoCommandHandler(
            mockOrdemServicoRepository.Object,
            mockServicoRepository.Object,
            mockItemEstoqueRepository.Object);

        var command = new AdicionarItemOrdemServicoCommand
        {
            IdOrdemServico = 1,
            IdServico = 5,
            ValorCobrado = 1000m,
            ItensNecessarios =
            [
                new AdicionarItemOrdemServicoCommand.ItemNecessario { IdItemEstoque = 10, Quantidade = 2m }
            ]
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Itens);
        Assert.Equal(1000m, result.ValorTotal);
        mockOrdemServicoRepository.Verify(r => r.UpdateAsync(ordem), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_WhenOrdemServicoNotFound()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((OrdemServicoAggregateRoot?)null);

        var handler = new AdicionarItemOrdemServicoCommandHandler(
            mockOrdemServicoRepository.Object,
            new Mock<IServicoRepository>().Object,
            new Mock<IItemEstoqueRepository>().Object);

        var command = new AdicionarItemOrdemServicoCommand { IdOrdemServico = 999, IdServico = 1, ValorCobrado = 100m };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Throws_WhenServicoNotFound()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        var mockServicoRepository = new Mock<IServicoRepository>();

        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(CriarOrdemEmDiagnostico());
        mockServicoRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ServicoAggregateRoot?)null);

        var handler = new AdicionarItemOrdemServicoCommandHandler(
            mockOrdemServicoRepository.Object,
            mockServicoRepository.Object,
            new Mock<IItemEstoqueRepository>().Object);

        var command = new AdicionarItemOrdemServicoCommand { IdOrdemServico = 1, IdServico = 999, ValorCobrado = 100m };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    private static OrdemServicoAggregateRoot CriarOrdemEmDiagnostico()
    {
        var ordem = OrdemServicoAggregateRoot.Criar(
            new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" },
            new VeiculoOrdemServico { Placa = "XYZ-9876", Marca = "Toyota", Modelo = "Corolla" });
        typeof(OrdemServicoAggregateRoot).GetProperty(nameof(OrdemServicoAggregateRoot.Id))!.SetValue(ordem, 1);
        ordem.EnviarParaDiagnostico();
        return ordem;
    }
}

public class AprovarServicosParcialmenteCommandHandlerTests
{
    [Fact]
    public async Task Handle_ApprovesSelectedServices_WhenIdsAreValid()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();

        var ordem = CriarOrdemAguardandoAprovacaoComDoisServicos();
        var idPrimeiro = ordem.Servicos.First().Id;

        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ordem);
        mockOrdemServicoRepository.Setup(r => r.UpdateAsync(It.IsAny<OrdemServicoAggregateRoot>())).Returns(Task.CompletedTask);

        var handler = new AprovarServicosParcialmenteCommandHandler(mockOrdemServicoRepository.Object);
        var command = new AprovarServicosParcialmenteCommand
        {
            IdOrdemServico = 1,
            IdServicosAprovados = [idPrimeiro]
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(StatusOrdemServico.EmDiagnostico, result.Status);
        Assert.Equal(StatusItemOrdemServico.Aprovado, ordem.Servicos.First(s => s.Id == idPrimeiro).Status);
        mockOrdemServicoRepository.Verify(r => r.UpdateAsync(ordem), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_WhenServiceIdDoesNotBelongToOrdem()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        var ordem = CriarOrdemAguardandoAprovacaoComDoisServicos();

        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ordem);

        var handler = new AprovarServicosParcialmenteCommandHandler(mockOrdemServicoRepository.Object);
        var command = new AprovarServicosParcialmenteCommand
        {
            IdOrdemServico = 1,
            IdServicosAprovados = [999]
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Throws_WhenOrdemNotFound()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((OrdemServicoAggregateRoot?)null);

        var handler = new AprovarServicosParcialmenteCommandHandler(mockOrdemServicoRepository.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new AprovarServicosParcialmenteCommand { IdOrdemServico = 999, IdServicosAprovados = [1] }, CancellationToken.None));
    }

    private static OrdemServicoAggregateRoot CriarOrdemAguardandoAprovacaoComDoisServicos()
    {
        var ordem = OrdemServicoAggregateRoot.Criar(
            new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" },
            new VeiculoOrdemServico { Placa = "XYZ-9876", Marca = "Toyota", Modelo = "Corolla" });
        typeof(OrdemServicoAggregateRoot).GetProperty(nameof(OrdemServicoAggregateRoot.Id))!.SetValue(ordem, 1);

        var catalogo = new ServicoCatalogo { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        ordem.EnviarParaDiagnostico();
        ordem.AdicionarItemServico("Troca", 200m, catalogo, []);
        ordem.AdicionarItemServico("Alinhamento", 150m, catalogo, []);
        ordem.FinalizarDiagnostico();
        return ordem;
    }
}
