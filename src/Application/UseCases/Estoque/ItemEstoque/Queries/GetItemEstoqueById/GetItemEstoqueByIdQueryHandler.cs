using Application.UseCases.Estoque.ItemEstoque.Responses;
using Application.Abstractions.Gateways;
using MediatR;

namespace Application.UseCases.Estoque.ItemEstoque.Queries.GetItemEstoqueById;

public class GetItemEstoqueByIdQueryHandler(IItemEstoqueGateway itemEstoqueGateway)
    : IRequestHandler<GetItemEstoqueByIdQuery, ItemEstoqueResponse?>
{
    public async Task<ItemEstoqueResponse?> Handle(GetItemEstoqueByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await itemEstoqueGateway.GetByIdAsync(request.Id);
        if (item == null)
        {
            return null;
        }

        return new ItemEstoqueResponse
        {
            Id = item.Id,
            Codigo = item.Codigo,
            Tipo = item.Tipo,
            Nome = item.Nome,
            UnidadeMedida = item.UnidadeMedida,
            PrecoVenda = item.PrecoVenda,
            Saldo = item.Saldo,
            SaldoReservado = item.SaldoReservado
        };
    }
}
