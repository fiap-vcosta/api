using Domain.Repositories;
using MediatR;

namespace Application.ItemEstoque.Commands.Handlers;

public class UpdateItemEstoqueCommandHandler(IItemEstoqueRepository itemEstoqueRepository)
    : IRequestHandler<UpdateItemEstoqueCommand, ItemEstoqueResponse>
{
    public async Task<ItemEstoqueResponse> Handle(UpdateItemEstoqueCommand request, CancellationToken cancellationToken)
    {
        var item = await itemEstoqueRepository.GetByIdAsync(request.Id);
        if (item == null)
        {
            throw new KeyNotFoundException($"Item de estoque com id {request.Id} não encontrado");
        }

        var existing = await itemEstoqueRepository.GetByCodigoAsync(request.Codigo);
        if (existing != null && existing.Id != item.Id)
        {
            throw new InvalidOperationException("Já existe um item de estoque com este código.");
        }

        item.Codigo = request.Codigo;
        item.Tipo = request.Tipo;
        item.Nome = request.Nome;
        item.UnidadeMedida = request.UnidadeMedida;
        item.PrecoVenda = request.PrecoVenda;
        item.Saldo = request.Saldo;
        item.SaldoReservado = request.SaldoReservado;

        await itemEstoqueRepository.UpdateAsync(item);

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
