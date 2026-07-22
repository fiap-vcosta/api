using Application.UseCases.Estoque.ItemEstoque.Responses;
using MediatR;

namespace Application.UseCases.Estoque.ItemEstoque.Queries.GetAllItensEstoque;

public class GetAllItemEstoqueQuery : IRequest<IEnumerable<ItemEstoqueResponse>>;
