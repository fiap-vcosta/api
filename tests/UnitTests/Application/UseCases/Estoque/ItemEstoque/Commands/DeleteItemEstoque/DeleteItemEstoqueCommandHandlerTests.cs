using Application.UseCases.Estoque.ItemEstoque.Commands.DeleteItemEstoque;
using Domain.Exceptions;
using Domain.Estoque.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Estoque.ItemEstoque.Commands.DeleteItemEstoque;

public class DeleteItemEstoqueCommandHandlerTests
{
    private readonly Mock<IItemEstoqueGateway> _mockGateway;
    private readonly DeleteItemEstoqueCommandHandler _handler;

    public DeleteItemEstoqueCommandHandlerTests()
    {
        _mockGateway = new Mock<IItemEstoqueGateway>();
        _handler = new DeleteItemEstoqueCommandHandler(_mockGateway.Object);
    }

    [Fact]
    public async Task Handle_DeletesItemEstoque_WhenItemExists()
    {
        _mockGateway.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new ItemEstoqueAggregateRoot { Id = 1, Codigo = "ITM-001" });
        _mockGateway.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        await _handler.Handle(new DeleteItemEstoqueCommand { Id = 1 }, CancellationToken.None);

        _mockGateway.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsDomainNotFoundException_WhenItemDoesNotExist()
    {
        _mockGateway.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ItemEstoqueAggregateRoot?)null);

        await Assert.ThrowsAsync<DomainNotFoundException>(() => _handler.Handle(new DeleteItemEstoqueCommand { Id = 999 }, CancellationToken.None));
    }
}
