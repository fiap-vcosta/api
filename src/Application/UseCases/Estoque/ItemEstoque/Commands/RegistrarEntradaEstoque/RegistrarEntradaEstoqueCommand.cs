using Application.UseCases.Estoque.ItemEstoque.Responses;
using MediatR;

namespace Application.UseCases.Estoque.ItemEstoque.Commands.RegistrarEntradaEstoque;

public class RegistrarEntradaEstoqueCommand : IRequest<ItemEstoqueResponse>
{
    public int IdItemEstoque { get; init; }
    public decimal QuantidadeRecebida { get; init; }
}
