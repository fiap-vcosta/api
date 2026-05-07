using Application.Estoque.ItemEstoque.Commands;
using Domain.Estoque.Repositories;
using MediatR;

namespace Application.Estoque.ItemEstoque.Queries.Handlers;

public class GetItemEstoqueByIdQueryHandler(IItemEstoqueRepository itemEstoqueRepository)
    : IRequestHandler<GetItemEstoqueByIdQuery, ItemEstoqueResponse?>
{
    public async Task<ItemEstoqueResponse?> Handle(GetItemEstoqueByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await itemEstoqueRepository.GetByIdAsync(request.Id);
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
