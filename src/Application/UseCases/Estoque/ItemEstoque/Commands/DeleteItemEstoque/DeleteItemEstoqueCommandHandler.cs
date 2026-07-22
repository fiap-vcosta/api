using Application.Abstractions.Gateways;
using Domain.Exceptions;

using MediatR;

namespace Application.UseCases.Estoque.ItemEstoque.Commands.DeleteItemEstoque;

public class DeleteItemEstoqueCommandHandler(IItemEstoqueGateway itemEstoqueGateway)
    : IRequestHandler<DeleteItemEstoqueCommand, Unit>
{
    public async Task<Unit> Handle(DeleteItemEstoqueCommand request, CancellationToken cancellationToken)
    {
        var item = await itemEstoqueGateway.GetByIdAsync(request.Id);
        if (item == null)
        {
            throw new DomainNotFoundException($"Item de estoque com id {request.Id} não encontrado");
        }

        await itemEstoqueGateway.DeleteAsync(request.Id);
        return Unit.Value;
    }
}
