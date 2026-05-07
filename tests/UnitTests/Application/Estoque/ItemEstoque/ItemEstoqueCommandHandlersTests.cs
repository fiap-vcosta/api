using Application.Estoque.ItemEstoque.Commands;
using Application.Estoque.ItemEstoque.Commands.Handlers;
using Domain.Estoque.Entities;
using Domain.Estoque.Repositories;
using Moq;

namespace UnitTests.Application.Estoque.ItemEstoque;

public class CreateItemEstoqueCommandHandlerTests
{
    private readonly Mock<IItemEstoqueRepository> _mockRepository;
    private readonly CreateItemEstoqueCommandHandler _handler;

    public CreateItemEstoqueCommandHandlerTests()
    {
        _mockRepository = new Mock<IItemEstoqueRepository>();
        _handler = new CreateItemEstoqueCommandHandler(_mockRepository.Object);
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

        _mockRepository.Setup(r => r.GetByCodigoAsync(command.Codigo))
            .ReturnsAsync((Domain.Estoque.Entities.ItemEstoque?)null);

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Domain.Estoque.Entities.ItemEstoque>()))
            .Callback<Domain.Estoque.Entities.ItemEstoque>(item => item.Id = 1)
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
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Domain.Estoque.Entities.ItemEstoque>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsInvalidOperationException_WhenCodigoAlreadyExists()
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

        _mockRepository.Setup(r => r.GetByCodigoAsync(command.Codigo))
            .ReturnsAsync(new Domain.Estoque.Entities.ItemEstoque { Id = 2, Codigo = command.Codigo, Nome = "Outro" });

        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}

public class UpdateItemEstoqueCommandHandlerTests
{
    private readonly Mock<IItemEstoqueRepository> _mockRepository;
    private readonly UpdateItemEstoqueCommandHandler _handler;

    public UpdateItemEstoqueCommandHandlerTests()
    {
        _mockRepository = new Mock<IItemEstoqueRepository>();
        _handler = new UpdateItemEstoqueCommandHandler(_mockRepository.Object);
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
            Saldo = 20.000m,
            SaldoReservado = 5.000m
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Domain.Estoque.Entities.ItemEstoque { Id = 1, Codigo = "ITM-001", Tipo = ItemTipo.Peca, Nome = "Filtro", UnidadeMedida = UnidadeMedida.Unidade, PrecoVenda = 50.00m, Saldo = 10.000m, SaldoReservado = 1.000m });
        _mockRepository.Setup(r => r.GetByCodigoAsync(command.Codigo))
            .ReturnsAsync((Domain.Estoque.Entities.ItemEstoque?)null);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Domain.Estoque.Entities.ItemEstoque>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("ITM-002", result.Codigo);
        Assert.Equal(ItemTipo.Insumo, result.Tipo);
        Assert.Equal("Óleo de Motor", result.Nome);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Domain.Estoque.Entities.ItemEstoque>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsKeyNotFoundException_WhenItemDoesNotExist()
    {
        var command = new UpdateItemEstoqueCommand
        {
            Id = 999,
            Codigo = "ITM-002",
            Tipo = ItemTipo.Insumo,
            Nome = "Óleo de Motor",
            UnidadeMedida = UnidadeMedida.Litro,
            PrecoVenda = 25.00m,
            Saldo = 20.000m,
            SaldoReservado = 5.000m
        };

        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Domain.Estoque.Entities.ItemEstoque?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}

public class DeleteItemEstoqueCommandHandlerTests
{
    private readonly Mock<IItemEstoqueRepository> _mockRepository;
    private readonly DeleteItemEstoqueCommandHandler _handler;

    public DeleteItemEstoqueCommandHandlerTests()
    {
        _mockRepository = new Mock<IItemEstoqueRepository>();
        _handler = new DeleteItemEstoqueCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_DeletesItemEstoque_WhenItemExists()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Domain.Estoque.Entities.ItemEstoque { Id = 1, Codigo = "ITM-001" });
        _mockRepository.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        await _handler.Handle(new DeleteItemEstoqueCommand { Id = 1 }, CancellationToken.None);

        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsKeyNotFoundException_WhenItemDoesNotExist()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Domain.Estoque.Entities.ItemEstoque?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(new DeleteItemEstoqueCommand { Id = 999 }, CancellationToken.None));
    }
}
