using Application.UseCases.Estoque.ItemEstoque.Commands.UpdateItemEstoque;
using Domain.Exceptions;
using Domain.Estoque.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Estoque.ItemEstoque.Commands.UpdateItemEstoque;

public class UpdateItemEstoqueCommandHandlerTests
{
    private readonly Mock<IItemEstoqueGateway> _mockGateway;
    private readonly UpdateItemEstoqueCommandHandler _handler;

    public UpdateItemEstoqueCommandHandlerTests()
    {
        _mockGateway = new Mock<IItemEstoqueGateway>();
        _handler = new UpdateItemEstoqueCommandHandler(_mockGateway.Object);
    }

    [Fact]
    public async Task Handle_UpdatesItemEstoque_WhenItemExists()
    {
        var command = new UpdateItemEstoqueCommand
        {
            Id = 1,
            Codigo = "ITM-002",
            Tipo = ItemTipo.Insumo,
            Nome = "Óleo de Motor",
            UnidadeMedida = UnidadeMedida.Litro,
            PrecoVenda = 25.00m,
        };

        _mockGateway.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new ItemEstoqueAggregateRoot { Id = 1, Codigo = "ITM-001", Tipo = ItemTipo.Peca, Nome = "Filtro", UnidadeMedida = UnidadeMedida.Unidade, PrecoVenda = 50.00m, Saldo = 10.000m, SaldoReservado = 1.000m });
        _mockGateway.Setup(r => r.GetByCodigoAsync(command.Codigo))
            .ReturnsAsync((ItemEstoqueAggregateRoot?)null);
        _mockGateway.Setup(r => r.UpdateAsync(It.IsAny<ItemEstoqueAggregateRoot>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("ITM-002", result.Codigo);
        Assert.Equal(ItemTipo.Insumo, result.Tipo);
        Assert.Equal("Óleo de Motor", result.Nome);
        _mockGateway.Verify(r => r.UpdateAsync(It.IsAny<ItemEstoqueAggregateRoot>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsDomainNotFoundException_WhenItemDoesNotExist()
    {
        var command = new UpdateItemEstoqueCommand
        {
            Id = 999,
            Codigo = "ITM-002",
            Tipo = ItemTipo.Insumo,
            Nome = "Óleo de Motor",
            UnidadeMedida = UnidadeMedida.Litro,
            PrecoVenda = 25.00m,
        };

        _mockGateway.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ItemEstoqueAggregateRoot?)null);

        await Assert.ThrowsAsync<DomainNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
