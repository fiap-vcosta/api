using Domain.Estoque.Repositories;
using MediatR;

namespace Application.Estoque.ItemEstoque.Commands.TravarItensNecessarios;

public class TravarItensNecessariosCommandHandler(IItemEstoqueRepository itemEstoqueRepository)
    : IRequestHandler<TravarItensNecessariosCommand, Unit>
{
    public async Task<Unit> Handle(TravarItensNecessariosCommand request, CancellationToken cancellationToken)
    {
        var item = await itemEstoqueRepository.GetByIdAsync(request.IdItemEstoque);
        if (item == null)
        {
            throw new KeyNotFoundException($"Item de estoque com id {request.IdItemEstoque} não encontrado");
        }

        item.TravarEstoque(request.QuantidadeNecessaria);
        await itemEstoqueRepository.UpdateAsync(item);
        
        return Unit.Value;
    }
}