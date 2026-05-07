using MediatR;

namespace Application.Administrativo.Cliente.Commands;

public class DeleteClienteCommand : IRequest<Unit>
{
    public int Id { get; init; }
}
