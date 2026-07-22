using Application.UseCases.Estoque.ItemEstoque.Commands.TravarItensNecessarios;
using Domain.Exceptions;
using Domain.Estoque.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Estoque.ItemEstoque.Commands.TravarItensNecessarios;

public class TravarItensNecessariosCommandHandlerTests
{
    [Fact]
    public async Task Handle_LocksStock_WhenItemExists()
    {
        // Arrange
        var mockGateway = new Mock<IItemEstoqueGateway>();
        var item = new ItemEstoqueAggregateRoot { Id = 1, Nome = "Pneu", Saldo = 10m, SaldoReservado = 0m };

        mockGateway.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
        mockGateway.Setup(r => r.UpdateAsync(It.IsAny<ItemEstoqueAggregateRoot>())).Returns(Task.CompletedTask);

        var handler = new TravarItensNecessariosCommandHandler(mockGateway.Object);

        // Act
        await handler.Handle(new TravarItensNecessariosCommand { IdItemEstoque = 1, QuantidadeNecessaria = 4m }, CancellationToken.None);

        // Assert
        Assert.Equal(4m, item.SaldoReservado);
        mockGateway.Verify(r => r.UpdateAsync(item), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_WhenItemNotFound()
    {
        // Arrange
        var mockGateway = new Mock<IItemEstoqueGateway>();
        mockGateway.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ItemEstoqueAggregateRoot?)null);

        var handler = new TravarItensNecessariosCommandHandler(mockGateway.Object);

        // Act & Assert
        await Assert.ThrowsAsync<DomainNotFoundException>(() =>
            handler.Handle(new TravarItensNecessariosCommand { IdItemEstoque = 999, QuantidadeNecessaria = 1m }, CancellationToken.None));
    }
}
