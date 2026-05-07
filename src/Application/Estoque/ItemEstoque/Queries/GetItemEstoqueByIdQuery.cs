using Application.Estoque.ItemEstoque.Commands;
using MediatR;

namespace Application.Estoque.ItemEstoque.Queries;

public class GetItemEstoqueByIdQuery : IRequest<ItemEstoqueResponse?>
{
    public int Id { get; init; }
}
