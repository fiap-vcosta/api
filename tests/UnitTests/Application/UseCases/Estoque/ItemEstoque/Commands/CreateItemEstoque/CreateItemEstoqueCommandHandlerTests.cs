using Application.UseCases.Estoque.ItemEstoque.Commands.CreateItemEstoque;
using Domain.Exceptions;
using Domain.Estoque.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Estoque.ItemEstoque.Commands.CreateItemEstoque;

public class CreateItemEstoqueCommandHandlerTests
{
    private readonly Mock<IItemEstoqueGateway> _mockGateway;
    private readonly CreateItemEstoqueCommandHandler _handler;

    public CreateItemEstoqueCommandHandlerTests()
    {
        _mockGateway = new Mock<IItemEstoqueGateway>();
        _handler = new CreateItemEstoqueCommandHandler(_mockGateway.Object);
    }

    [Fact]
    public async Task Handle_CreatesItemEstoque_WhenCommandIsValid()
    {
        var command = new CreateItemEstoqueCommand
        {
            Codigo = "ITM-001",
            Tipo = ItemTipo.Peca,
            Nome = "Filtro de Óleo",
            UnidadeMedida = UnidadeMedida.Unidade,
            PrecoVenda = 55.50m,
            Saldo = 10.000m,
            SaldoReservado = 2.000m
        };

        _mockGateway.Setup(r => r.GetByCodigoAsync(command.Codigo))
            .ReturnsAsync((ItemEstoqueAggregateRoot?)null);

        _mockGateway.Setup(r => r.CreateAsync(It.IsAny<ItemEstoqueAggregateRoot>()))
            .Callback<ItemEstoqueAggregateRoot>(item => item.Id = 1)
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("ITM-001", result.Codigo);
        Assert.Equal(ItemTipo.Peca, result.Tipo);
        Assert.Equal("Filtro de Óleo", result.Nome);
        Assert.Equal(UnidadeMedida.Unidade, result.UnidadeMedida);
        Assert.Equal(55.50m, result.PrecoVenda);
        Assert.Equal(10.000m, result.Saldo);
        Assert.Equal(2.000m, result.SaldoReservado);
        _mockGateway.Verify(r => r.CreateAsync(It.IsAny<ItemEstoqueAggregateRoot>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsBusinessRuleException_WhenCodigoAlreadyExists()
    {
        var command = new CreateItemEstoqueCommand
        {
            Codigo = "ITM-001",
            Tipo = ItemTipo.Peca,
            Nome = "Filtro de Óleo",
            UnidadeMedida = UnidadeMedida.Unidade,
            PrecoVenda = 55.50m,
            Saldo = 10.000m,
            SaldoReservado = 2.000m
        };

        _mockGateway.Setup(r => r.GetByCodigoAsync(command.Codigo))
            .ReturnsAsync(new ItemEstoqueAggregateRoot { Id = 2, Codigo = command.Codigo, Nome = "Outro" });

        await Assert.ThrowsAsync<BusinessRuleException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
