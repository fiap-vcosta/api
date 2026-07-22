using Application.UseCases.Estoque.ItemEstoque.Responses;
using Application.Abstractions.Gateways;
using MediatR;

namespace Application.UseCases.Estoque.ItemEstoque.Queries.GetAllItensEstoque;

public class GetAllItemEstoqueQueryHandler(IItemEstoqueGateway itemEstoqueGateway)
    : IRequestHandler<GetAllItemEstoqueQuery, IEnumerable<ItemEstoqueResponse>>
{
    public async Task<IEnumerable<ItemEstoqueResponse>> Handle(GetAllItemEstoqueQuery request, CancellationToken cancellationToken)
    {
        var itens = await itemEstoqueGateway.GetAllAsync();
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
