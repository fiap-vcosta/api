using Application.Estoque.ItemEstoque.Commands.CreateItemEstoque;
using Application.Estoque.ItemEstoque.Queries.GetAllItensEstoque;
using Application.Estoque.ItemEstoque.Queries.GetItemEstoqueById;
using Domain.Estoque.Entities;
using Domain.Estoque.Repositories;
using Moq;

namespace UnitTests.Application.Estoque.ItemEstoque;

public class ItemEstoqueQueryHandlersTests
{
    [Fact]
    public async Task GetById_ReturnsNull_WhenItemNotFound()
    {
        // Arrange
        var mockRepository = new Mock<IItemEstoqueRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ItemEstoqueAggregateRoot?)null);
        var handler = new GetItemEstoqueByIdQueryHandler(mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetItemEstoqueByIdQuery { Id = 999 }, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetById_ReturnsMappedResponse_WhenItemExists()
    {
        // Arrange
        var mockRepository = new Mock<IItemEstoqueRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ItemEstoqueAggregateRoot
        {
            Id = 1,
            Codigo = "ITM-001",
            Tipo = ItemTipo.Peca,
            Nome = "Filtro",
            UnidadeMedida = UnidadeMedida.Unidade,
            PrecoVenda = 10m,
            Saldo = 5m,
            SaldoReservado = 1m
        });
        var handler = new GetItemEstoqueByIdQueryHandler(mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetItemEstoqueByIdQuery { Id = 1 }, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ITM-001", result.Codigo);
        Assert.Equal(5m, result.Saldo);
    }

    [Fact]
    public async Task GetAll_ReturnsMappedList()
    {
        // Arrange
        var mockRepository = new Mock<IItemEstoqueRepository>();
        mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ItemEstoqueAggregateRoot>
        {
            new() { Id = 1, Codigo = "A", Nome = "Item A", Tipo = ItemTipo.Peca, UnidadeMedida = UnidadeMedida.Unidade },
            new() { Id = 2, Codigo = "B", Nome = "Item B", Tipo = ItemTipo.Insumo, UnidadeMedida = UnidadeMedida.Litro }
        });
        var handler = new GetAllItemEstoqueQueryHandler(mockRepository.Object);

        // Act
        var result = (await handler.Handle(new GetAllItemEstoqueQuery(), CancellationToken.None)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("A", result[0].Codigo);
        Assert.Equal("B", result[1].Codigo);
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenRepositoryIsEmpty()
    {
        // Arrange
        var mockRepository = new Mock<IItemEstoqueRepository>();
        mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ItemEstoqueAggregateRoot>());
        var handler = new GetAllItemEstoqueQueryHandler(mockRepository.Object);

        // Act
        var result = await handler.Handle(new GetAllItemEstoqueQuery(), CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}
