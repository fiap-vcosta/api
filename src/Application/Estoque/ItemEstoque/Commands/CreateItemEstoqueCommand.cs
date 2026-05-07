using Domain.Estoque.Entities;
using MediatR;

namespace Application.Estoque.ItemEstoque.Commands;

public class CreateItemEstoqueCommand : IRequest<ItemEstoqueResponse>
{
    public string Codigo { get; init; } = string.Empty;
    public ItemTipo Tipo { get; init; }
    public string Nome { get; init; } = string.Empty;
    public UnidadeMedida UnidadeMedida { get; init; }
    public decimal PrecoVenda { get; init; }
    public decimal Saldo { get; init; }
    public decimal SaldoReservado { get; init; }
}

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
}
