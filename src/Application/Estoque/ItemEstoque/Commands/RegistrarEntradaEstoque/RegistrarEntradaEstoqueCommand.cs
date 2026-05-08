using Application.Estoque.ItemEstoque.Commands.CreateItemEstoque;
using MediatR;

namespace Application.Estoque.ItemEstoque.Commands.RegistrarEntradaEstoque;

public class RegistrarEntradaEstoqueCommand : IRequest<ItemEstoqueResponse>
{
    public int IdItemEstoque { get; init; }
    public decimal QuantidadeRecebida { get; init; }
}
