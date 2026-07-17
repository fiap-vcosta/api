using Application.UseCases.Estoque.ItemEstoque.Responses;
using MediatR;

namespace Application.UseCases.Estoque.ItemEstoque.Queries.GetItemEstoqueById;

public class GetItemEstoqueByIdQuery : IRequest<ItemEstoqueResponse?>
{
    public int Id { get; init; }
}
