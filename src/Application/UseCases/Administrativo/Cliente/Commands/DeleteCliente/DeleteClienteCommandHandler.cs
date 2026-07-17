using Application.Abstractions.Gateways;
using Domain.Exceptions;

using MediatR;

namespace Application.UseCases.Administrativo.Cliente.Commands.DeleteCliente;

public class DeleteClienteCommandHandler(IClienteGateway clienteGateway)
    : IRequestHandler<DeleteClienteCommand, Unit>
{
    public async Task<Unit> Handle(DeleteClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await clienteGateway.GetByIdAsync(request.Id);
        if (cliente == null)
        {
            throw new DomainNotFoundException($"Cliente com id {request.Id} não encontrado");
        }

        await clienteGateway.DeleteAsync(request.Id);
        return Unit.Value;
    }
}
