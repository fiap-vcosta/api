using Application.Abstractions.Gateways;
using Domain.Exceptions;

using MediatR;

namespace Application.UseCases.Administrativo.Veiculo.Commands.DeleteVeiculo;

public class DeleteVeiculoCommandHandler(IVeiculoGateway veiculoGateway)
    : IRequestHandler<DeleteVeiculoCommand, Unit>
{
    public async Task<Unit> Handle(DeleteVeiculoCommand request, CancellationToken cancellationToken)
    {
        var veiculo = await veiculoGateway.GetByIdAsync(request.Id);
        if (veiculo == null)
        {
            throw new DomainNotFoundException($"Veículo com id {request.Id} não encontrado");
        }

        await veiculoGateway.DeleteAsync(request.Id);
        return Unit.Value;
    }
}
