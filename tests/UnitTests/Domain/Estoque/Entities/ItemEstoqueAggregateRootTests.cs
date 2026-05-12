using Domain.Estoque.Entities;

namespace UnitTests.Domain.Estoque.Entities;

public class ItemEstoqueAggregateRootTests
{
    [Fact]
    public void GetSaldoDisponivel_ReturnsSaldoMinusSaldoReservado()
    {
        // Arrange
        var item = new ItemEstoqueAggregateRoot
        {
            Id = 1,
            Codigo = "ITM-01",
            Tipo = ItemTipo.Peca,
            Nome = "Filtro de óleo",
            UnidadeMedida = UnidadeMedida.Unidade,
            PrecoVenda = 45.5m,
            Saldo = 10m,
            SaldoReservado = 4m
        };

        // Act
        var saldoDisponivel = item.SaldoDisponivel;

        // Assert
        Assert.Equal(6m, saldoDisponivel);
    }

    [Fact]
    public void RegisterEntryLockAndConfirmUsage_UpdatesStockAmounts()
    {
        // Arrange
        var item = new ItemEstoqueAggregateRoot
        {
            Saldo = 5m,
            SaldoReservado = 2m
        };

        // Act
        item.RegistrarEntradaEstoque(3m);
        item.TravarEstoque(4m);
        item.ConfirmarUtilizacao(4m);

        // Assert
        Assert.Equal(4m, item.Saldo);
        Assert.Equal(2m, item.SaldoReservado);
        Assert.Equal(2m, item.SaldoDisponivel);
    }
}
