using MediatR;

namespace Application.Estoque.ItemEstoque.Commands.RegistrarEntradaEstoque;

public class RegistrarEntradaEstoqueCommand : IRequest<Unit>
{
    public int IdItemEstoque { get; init; }
    public decimal QuantidadeRecebida { get; init; }
}
