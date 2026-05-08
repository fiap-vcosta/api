using MediatR;

namespace Application.Administrativo.Cliente.Commands.DeleteCliente;

public class DeleteClienteCommand : IRequest<Unit>
{
    public int Id { get; init; }
}
