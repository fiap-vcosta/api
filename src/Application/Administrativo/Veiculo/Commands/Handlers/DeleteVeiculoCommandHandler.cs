using Domain.Administrativo.Repositories;
using MediatR;

namespace Application.Administrativo.Veiculo.Commands.Handlers;

public class DeleteVeiculoCommandHandler(IVeiculoRepository veiculoRepository)
    : IRequestHandler<DeleteVeiculoCommand, Unit>
{
    public async Task<Unit> Handle(DeleteVeiculoCommand request, CancellationToken cancellationToken)
    {
        var veiculo = await veiculoRepository.GetByIdAsync(request.Id);
        if (veiculo == null)
        {
            throw new KeyNotFoundException($"Veículo com id {request.Id} não encontrado");
        }

        await veiculoRepository.DeleteAsync(request.Id);
        return Unit.Value;
    }
}
