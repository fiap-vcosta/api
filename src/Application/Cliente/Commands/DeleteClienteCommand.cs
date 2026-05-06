using MediatR;

namespace Application.Cliente.Commands;

public class DeleteClienteCommand : IRequest<Unit>
{
    public int Id { get; init; }
}
