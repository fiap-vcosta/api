namespace Api.Controllers.ItemEstoque.UpdateItemEstoque;

public class UpdateItemEstoqueRequest
{
    public string Codigo { get; init; } = string.Empty;
    public int Tipo { get; init; }
    public string Nome { get; init; } = string.Empty;
    public int UnidadeMedida { get; init; }
    public decimal PrecoVenda { get; init; }
    public decimal Saldo { get; init; }
    public decimal SaldoReservado { get; init; }
}
