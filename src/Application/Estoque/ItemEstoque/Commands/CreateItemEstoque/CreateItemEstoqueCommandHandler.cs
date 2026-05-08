using Domain.Estoque.Repositories;
using MediatR;

namespace Application.Estoque.ItemEstoque.Commands.CreateItemEstoque;

public class CreateItemEstoqueCommandHandler(IItemEstoqueRepository itemEstoqueRepository)
    : IRequestHandler<CreateItemEstoqueCommand, ItemEstoqueResponse>
{
    public async Task<ItemEstoqueResponse> Handle(CreateItemEstoqueCommand request, CancellationToken cancellationToken)
    {
        var existing = await itemEstoqueRepository.GetByCodigoAsync(request.Codigo);
        if (existing != null)
        {
            throw new InvalidOperationException("Já existe um item de estoque com este código.");
        }

        var item = new Domain.Estoque.Entities.ItemEstoqueAggregateRoot
        {
            Codigo = request.Codigo,
            Tipo = request.Tipo,
            Nome = request.Nome,
            UnidadeMedida = request.UnidadeMedida,
            PrecoVenda = request.PrecoVenda,
            Saldo = request.Saldo,
            SaldoReservado = request.SaldoReservado
        };

        await itemEstoqueRepository.CreateAsync(item);

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
