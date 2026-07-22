using Domain.Estoque.Entities;

namespace Application.UseCases.Estoque.ItemEstoque.Responses;

public class ItemEstoqueResponse
{
    public int Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public ItemTipo Tipo { get; init; }
    public string Nome { get; init; } = string.Empty;
    public UnidadeMedida UnidadeMedida { get; init; }
    public decimal PrecoVenda { get; init; }
    public decimal Saldo { get; init; }
    public decimal SaldoReservado { get; init; }

    public static ItemEstoqueResponse FromAggregateRoot(ItemEstoqueAggregateRoot itemEstoque)
    {
        return new ItemEstoqueResponse()
        {
            Id = itemEstoque.Id,
            Codigo = itemEstoque.Codigo,
            Tipo = itemEstoque.Tipo,
            Nome = itemEstoque.Nome,
            UnidadeMedida = itemEstoque.UnidadeMedida,
            PrecoVenda = itemEstoque.PrecoVenda,
            Saldo = itemEstoque.Saldo,
            SaldoReservado = itemEstoque.SaldoReservado,
        };
    }
}
