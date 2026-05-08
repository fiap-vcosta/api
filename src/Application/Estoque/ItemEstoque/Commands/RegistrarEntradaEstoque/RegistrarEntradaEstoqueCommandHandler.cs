using Application.Estoque.ItemEstoque.Commands.CreateItemEstoque;
using Domain.Estoque.Entities;
using Domain.Estoque.Events;
using Domain.Estoque.Repositories;
using MediatR;

namespace Application.Estoque.ItemEstoque.Commands.RegistrarEntradaEstoque;

public class RegistrarEntradaEstoqueCommandHandler(IItemEstoqueRepository itemEstoqueRepository, IMediator mediator)
    : IRequestHandler<RegistrarEntradaEstoqueCommand, ItemEstoqueResponse>
{
    public async Task<ItemEstoqueResponse> Handle(RegistrarEntradaEstoqueCommand request, CancellationToken cancellationToken)
    {
        var item = await itemEstoqueRepository.GetByIdAsync(request.IdItemEstoque);
        if (item == null)
        {
            throw new KeyNotFoundException($"Item de estoque com id {request.IdItemEstoque} não encontrado");
        }
        
        item.RegistrarEntradaEstoque(request.QuantidadeRecebida);

        await itemEstoqueRepository.UpdateAsync(item);
        await mediator.Publish(new ChegadaDeItensRegistradaEvent(item), cancellationToken);

        return ItemEstoqueResponse.FromAggregateRoot(item);
    }
}
