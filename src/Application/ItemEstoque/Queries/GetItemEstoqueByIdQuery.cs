using MediatR;
using Application.ItemEstoque.Commands;

namespace Application.ItemEstoque.Queries;

public class GetItemEstoqueByIdQuery : IRequest<ItemEstoqueResponse?>
{
    public int Id { get; init; }
}
