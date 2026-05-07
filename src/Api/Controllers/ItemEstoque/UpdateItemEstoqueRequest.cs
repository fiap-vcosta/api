namespace Api.Controllers.ItemEstoque;

public class UpdateItemEstoqueRequest
{
    public string Codigo { get; set; } = string.Empty;
    public Domain.Entities.ItemTipo Tipo { get; set; }
    public string Nome { get; set; } = string.Empty;
    public Domain.Entities.UnidadeMedida UnidadeMedida { get; set; }
    public decimal PrecoVenda { get; set; }
    public decimal Saldo { get; set; }
    public decimal SaldoReservado { get; set; }
}
