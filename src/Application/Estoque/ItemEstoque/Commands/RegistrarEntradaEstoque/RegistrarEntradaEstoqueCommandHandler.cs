using Domain.Estoque.Events;
using Domain.Estoque.Repositories;
using MediatR;

namespace Application.Estoque.ItemEstoque.Commands.RegistrarEntradaEstoque;

public class RegistrarEntradaEstoqueCommandHandler(IItemEstoqueRepository itemEstoqueRepository, IMediator mediator)
    : IRequestHandler<RegistrarEntradaEstoqueCommand, Unit>
{
    public async Task<Unit> Handle(RegistrarEntradaEstoqueCommand request, CancellationToken cancellationToken)
    {
        var item = await itemEstoqueRepository.GetByIdAsync(request.IdItemEstoque);
        if (item == null)
        {
            throw new KeyNotFoundException($"Item de estoque com id {request.IdItemEstoque} não encontrado");
        }
        
        item.RegistrarEntradaEstoque(request.QuantidadeRecebida);

        await itemEstoqueRepository.UpdateAsync(item);
        await mediator.Publish(new ChegadaDeItensRegistradaEvent(item), cancellationToken);
        
        return Unit.Value;
    }
}