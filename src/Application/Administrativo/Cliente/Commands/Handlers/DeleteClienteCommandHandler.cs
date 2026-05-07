using Domain.Administrativo.Repositories;
using MediatR;

namespace Application.Administrativo.Cliente.Commands.Handlers;

public class DeleteClienteCommandHandler(IClienteRepository clienteRepository)
    : IRequestHandler<DeleteClienteCommand, Unit>
{
    public async Task<Unit> Handle(DeleteClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await clienteRepository.GetByIdAsync(request.Id);
        if (cliente == null)
        {
            throw new KeyNotFoundException($"Cliente com id {request.Id} não encontrado");
        }

        await clienteRepository.DeleteAsync(request.Id);
        return Unit.Value;
    }
}
