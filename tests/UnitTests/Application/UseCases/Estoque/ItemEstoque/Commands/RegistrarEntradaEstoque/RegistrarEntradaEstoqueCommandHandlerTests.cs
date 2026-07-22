using Application.Abstractions.Events;
using Domain.Exceptions;
using Application.UseCases.Estoque.ItemEstoque.Commands.RegistrarEntradaEstoque;
using Domain.Estoque.Entities;
using Domain.Estoque.Events;
using Application.Abstractions.Gateways;
using MediatR;
using Moq;

namespace UnitTests.Application.UseCases.Estoque.ItemEstoque.Commands.RegistrarEntradaEstoque;

public class RegistrarEntradaEstoqueCommandHandlerTests
{
    [Fact]
    public async Task Handle_RegistersEntryAndPublishesEvent_WhenItemExists()
    {
        // Arrange
        var mockGateway = new Mock<IItemEstoqueGateway>();
        var mockMediator = new Mock<IMediator>();
        var item = new ItemEstoqueAggregateRoot { Id = 1, Nome = "Filtro", Saldo = 10m };

        mockGateway.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
        mockGateway.Setup(r => r.UpdateAsync(It.IsAny<ItemEstoqueAggregateRoot>())).Returns(Task.CompletedTask);
        mockMediator
            .Setup(m => m.Publish(It.IsAny<DomainEventNotification<ChegadaDeItensRegistradaEvent>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new RegistrarEntradaEstoqueCommandHandler(mockGateway.Object, mockMediator.Object);

        // Act
        var result = await handler.Handle(
            new RegistrarEntradaEstoqueCommand { IdItemEstoque = 1, QuantidadeRecebida = 50m },
            CancellationToken.None);

        // Assert
        Assert.Equal(60m, result.Saldo);
        mockMediator.Verify(
            m => m.Publish(It.Is<DomainEventNotification<ChegadaDeItensRegistradaEvent>>(n => n.DomainEvent.ItemEstoqueAggregateRoot.Id == 1), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_WhenItemNotFound()
    {
        // Arrange
        var mockGateway = new Mock<IItemEstoqueGateway>();
        mockGateway.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ItemEstoqueAggregateRoot?)null);

        var handler = new RegistrarEntradaEstoqueCommandHandler(mockGateway.Object, new Mock<IMediator>().Object);

        // Act & Assert
        await Assert.ThrowsAsync<DomainNotFoundException>(() =>
            handler.Handle(new RegistrarEntradaEstoqueCommand { IdItemEstoque = 999, QuantidadeRecebida = 1m }, CancellationToken.None));
    }
}
