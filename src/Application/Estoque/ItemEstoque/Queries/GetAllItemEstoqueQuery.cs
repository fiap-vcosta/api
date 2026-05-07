using Application.Estoque.ItemEstoque.Commands;
using MediatR;

namespace Application.Estoque.ItemEstoque.Queries;

public class GetAllItemEstoqueQuery : IRequest<IEnumerable<ItemEstoqueResponse>>;
