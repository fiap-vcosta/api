namespace Domain.Estoque.Entities;

public enum ItemTipo
{
    Peca,
    Insumo
}

public enum UnidadeMedida
{
    Unidade,
    Jogo,
    Par,
    Litro,
    Kg,
    mL,
    Frasco
}

public class ItemEstoqueAggregateRoot
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public ItemTipo Tipo { get; set; }
    public string Nome { get; set; } = string.Empty;
    public UnidadeMedida UnidadeMedida { get; set; }
    public decimal PrecoVenda { get; set; }
    public decimal Saldo { get; set; }
    public decimal SaldoReservado { get; set; }

    public void RegistrarEntradaEstoque(Decimal quantidade)
    {
        Saldo += quantidade;
    }

    public void TravarEstoque(Decimal quantidade)
    {
        SaldoReservado += quantidade;
    }
}
