using Application.Estoque.ItemEstoque.Commands;
using Domain.Estoque.Repositories;
using MediatR;

namespace Application.Estoque.ItemEstoque.Queries.Handlers;

public class GetAllItemEstoqueQueryHandler(IItemEstoqueRepository itemEstoqueRepository)
    : IRequestHandler<GetAllItemEstoqueQuery, IEnumerable<ItemEstoqueResponse>>
{
    public async Task<IEnumerable<ItemEstoqueResponse>> Handle(GetAllItemEstoqueQuery request, CancellationToken cancellationToken)
    {
        var itens = await itemEstoqueRepository.GetAllAsync();
        return itens.Select(item => new ItemEstoqueResponse
        {
            Id = item.Id,
            Codigo = item.Codigo,
            Tipo = item.Tipo,
            Nome = item.Nome,
            UnidadeMedida = item.UnidadeMedida,
            PrecoVenda = item.PrecoVenda,
            Saldo = item.Saldo,
            SaldoReservado = item.SaldoReservado
        }).ToList();
    }
}
