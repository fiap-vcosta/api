using Application.Administrativo.Cliente.Commands;
using MediatR;

namespace Application.Administrativo.Cliente.Queries;

public class GetClienteByIdQuery : IRequest<ClienteResponse?>
{
    public int Id { get; init; }
}
