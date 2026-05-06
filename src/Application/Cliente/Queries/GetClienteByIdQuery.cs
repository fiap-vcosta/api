using Application.Cliente.Commands;
using MediatR;

namespace Application.Cliente.Queries;

public class GetClienteByIdQuery : IRequest<ClienteResponse?>
{
    public int Id { get; init; }
}
