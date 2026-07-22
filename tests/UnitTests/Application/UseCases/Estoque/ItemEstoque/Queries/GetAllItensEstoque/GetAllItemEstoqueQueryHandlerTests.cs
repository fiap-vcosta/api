using Application.UseCases.Estoque.ItemEstoque.Queries.GetAllItensEstoque;
using Domain.Estoque.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Estoque.ItemEstoque.Queries.GetAllItensEstoque;

public class GetAllItemEstoqueQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsMappedList()
    {
        // Arrange
        var mockGateway = new Mock<IItemEstoqueGateway>();
        mockGateway.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ItemEstoqueAggregateRoot>
        {
            new() { Id = 1, Codigo = "A", Nome = "Item A", Tipo = ItemTipo.Peca, UnidadeMedida = UnidadeMedida.Unidade },
            new() { Id = 2, Codigo = "B", Nome = "Item B", Tipo = ItemTipo.Insumo, UnidadeMedida = UnidadeMedida.Litro }
        });
        var handler = new GetAllItemEstoqueQueryHandler(mockGateway.Object);

        // Act
        var result = (await handler.Handle(new GetAllItemEstoqueQuery(), CancellationToken.None)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("A", result[0].Codigo);
        Assert.Equal("B", result[1].Codigo);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenGatewayIsEmpty()
    {
        // Arrange
        var mockGateway = new Mock<IItemEstoqueGateway>();
        mockGateway.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ItemEstoqueAggregateRoot>());
        var handler = new GetAllItemEstoqueQueryHandler(mockGateway.Object);

        // Act
        var result = await handler.Handle(new GetAllItemEstoqueQuery(), CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}
