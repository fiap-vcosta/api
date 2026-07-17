using Application.UseCases.OrdemServico.Commands.AdicionarItemOrdemServico;
using Domain.Exceptions;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Domain.Estoque.Entities;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.ValueObjects;
using Moq;

namespace UnitTests.Application.UseCases.OrdemServico.Commands.AdicionarItemOrdemServico;

public class AdicionarItemOrdemServicoCommandHandlerTests
{
    [Fact]
    public async Task Handle_AddsServiceItem_WhenCommandIsValid()
    {
        // Arrange
        var mockOrdemServicoGateway = new Mock<IOrdemServicoGateway>();
        var mockServicoGateway = new Mock<IServicoGateway>();
        var mockItemEstoqueGateway = new Mock<IItemEstoqueGateway>();

        var ordem = CriarOrdemEmDiagnostico();
        var servico = new ServicoAggregateRoot { Id = 5, Codigo = "OLE-001", Nome = "Troca de Óleo", PrecoPadrao = 150m, Ativo = true };
        var itemEstoque = new ItemEstoqueAggregateRoot { Id = 10, Codigo = "FLT-001", Nome = "Filtro", UnidadeMedida = UnidadeMedida.Unidade };

        mockOrdemServicoGateway.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ordem);
        mockServicoGateway.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(servico);
        mockItemEstoqueGateway.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(itemEstoque);
        mockOrdemServicoGateway.Setup(r => r.UpdateAsync(It.IsAny<OrdemServicoAggregateRoot>())).Returns(Task.CompletedTask);

        var handler = new AdicionarItemOrdemServicoCommandHandler(
            mockOrdemServicoGateway.Object,
            mockServicoGateway.Object,
            mockItemEstoqueGateway.Object);

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
        mockOrdemServicoGateway.Verify(r => r.UpdateAsync(ordem), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_WhenOrdemServicoNotFound()
    {
        // Arrange
        var mockOrdemServicoGateway = new Mock<IOrdemServicoGateway>();
        mockOrdemServicoGateway.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((OrdemServicoAggregateRoot?)null);

        var handler = new AdicionarItemOrdemServicoCommandHandler(
            mockOrdemServicoGateway.Object,
            new Mock<IServicoGateway>().Object,
            new Mock<IItemEstoqueGateway>().Object);

        var command = new AdicionarItemOrdemServicoCommand { IdOrdemServico = 999, IdServico = 1, ValorCobrado = 100m };

        // Act & Assert
        await Assert.ThrowsAsync<DomainNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Throws_WhenServicoNotFound()
    {
        // Arrange
        var mockOrdemServicoGateway = new Mock<IOrdemServicoGateway>();
        var mockServicoGateway = new Mock<IServicoGateway>();

        mockOrdemServicoGateway.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(CriarOrdemEmDiagnostico());
        mockServicoGateway.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ServicoAggregateRoot?)null);

        var handler = new AdicionarItemOrdemServicoCommandHandler(
            mockOrdemServicoGateway.Object,
            mockServicoGateway.Object,
            new Mock<IItemEstoqueGateway>().Object);

        var command = new AdicionarItemOrdemServicoCommand { IdOrdemServico = 1, IdServico = 999, ValorCobrado = 100m };

        // Act & Assert
        await Assert.ThrowsAsync<DomainNotFoundException>(() => handler.Handle(command, CancellationToken.None));
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
