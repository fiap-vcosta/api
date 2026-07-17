using Application.UseCases.Estoque.ItemEstoque.Queries.GetItemEstoqueById;
using Domain.Estoque.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Estoque.ItemEstoque.Queries.GetItemEstoqueById;

public class GetItemEstoqueByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsNull_WhenItemNotFound()
    {
        // Arrange
        var mockGateway = new Mock<IItemEstoqueGateway>();
        mockGateway.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ItemEstoqueAggregateRoot?)null);
        var handler = new GetItemEstoqueByIdQueryHandler(mockGateway.Object);

        // Act
        var result = await handler.Handle(new GetItemEstoqueByIdQuery { Id = 999 }, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ReturnsMappedResponse_WhenItemExists()
    {
        // Arrange
        var mockGateway = new Mock<IItemEstoqueGateway>();
        mockGateway.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ItemEstoqueAggregateRoot
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
        var handler = new GetItemEstoqueByIdQueryHandler(mockGateway.Object);

        // Act
        var result = await handler.Handle(new GetItemEstoqueByIdQuery { Id = 1 }, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ITM-001", result.Codigo);
        Assert.Equal(5m, result.Saldo);
    }
}
