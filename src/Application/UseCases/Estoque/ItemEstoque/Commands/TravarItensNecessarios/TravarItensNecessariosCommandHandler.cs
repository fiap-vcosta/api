using Application.Abstractions.Gateways;
using Domain.Exceptions;

using MediatR;

namespace Application.UseCases.Estoque.ItemEstoque.Commands.TravarItensNecessarios;

public class TravarItensNecessariosCommandHandler(IItemEstoqueGateway itemEstoqueGateway)
    : IRequestHandler<TravarItensNecessariosCommand, Unit>
{
    public async Task<Unit> Handle(TravarItensNecessariosCommand request, CancellationToken cancellationToken)
    {
        var item = await itemEstoqueGateway.GetByIdAsync(request.IdItemEstoque);
        if (item == null)
        {
            throw new DomainNotFoundException($"Item de estoque com id {request.IdItemEstoque} não encontrado");
        }

        item.TravarEstoque(request.QuantidadeNecessaria);
        await itemEstoqueGateway.UpdateAsync(item);
        
        return Unit.Value;
    }
}