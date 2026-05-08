using Application.Estoque.ItemEstoque.Commands.CreateItemEstoque;
using MediatR;

namespace Application.Estoque.ItemEstoque.Queries.GetItemEstoqueById;

public class GetItemEstoqueByIdQuery : IRequest<ItemEstoqueResponse?>
{
    public int Id { get; init; }
}
