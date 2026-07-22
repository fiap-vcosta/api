using Application.Abstractions.Events;
using Domain.Exceptions;

using Application.UseCases.Estoque.ItemEstoque.Responses;
using Domain.Estoque.Events;
using Application.Abstractions.Gateways;
using MediatR;

namespace Application.UseCases.Estoque.ItemEstoque.Commands.RegistrarEntradaEstoque;

public class RegistrarEntradaEstoqueCommandHandler(IItemEstoqueGateway itemEstoqueGateway, IMediator mediator)
    : IRequestHandler<RegistrarEntradaEstoqueCommand, ItemEstoqueResponse>
{
    public async Task<ItemEstoqueResponse> Handle(RegistrarEntradaEstoqueCommand request, CancellationToken cancellationToken)
    {
        var item = await itemEstoqueGateway.GetByIdAsync(request.IdItemEstoque);
        if (item == null)
        {
            throw new DomainNotFoundException($"Item de estoque com id {request.IdItemEstoque} não encontrado");
        }
        
        item.RegistrarEntradaEstoque(request.QuantidadeRecebida);

        await itemEstoqueGateway.UpdateAsync(item);
        await mediator.Publish(new DomainEventNotification<ChegadaDeItensRegistradaEvent>(new ChegadaDeItensRegistradaEvent(item)), cancellationToken);

        return ItemEstoqueResponse.FromAggregateRoot(item);
    }
}
