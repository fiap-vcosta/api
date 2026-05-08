using Application.Estoque.ItemEstoque.Commands.CreateItemEstoque;
using MediatR;

namespace Application.Estoque.ItemEstoque.Queries.GetAllItensEstoque;

public class GetAllItemEstoqueQuery : IRequest<IEnumerable<ItemEstoqueResponse>>;
