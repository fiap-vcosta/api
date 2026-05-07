using Domain.Estoque.Repositories;
using MediatR;

namespace Application.Estoque.ItemEstoque.Commands.Handlers;

public class DeleteItemEstoqueCommandHandler(IItemEstoqueRepository itemEstoqueRepository)
    : IRequestHandler<DeleteItemEstoqueCommand, Unit>
{
    public async Task<Unit> Handle(DeleteItemEstoqueCommand request, CancellationToken cancellationToken)
    {
        var item = await itemEstoqueRepository.GetByIdAsync(request.Id);
        if (item == null)
        {
            throw new KeyNotFoundException($"Item de estoque com id {request.Id} não encontrado");
        }

        await itemEstoqueRepository.DeleteAsync(request.Id);
        return Unit.Value;
    }
}
