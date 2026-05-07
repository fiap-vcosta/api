using MediatR;
using Application.ItemEstoque.Commands;

namespace Application.ItemEstoque.Queries;

public class GetAllItemEstoqueQuery : IRequest<IEnumerable<ItemEstoqueResponse>>
{
}
