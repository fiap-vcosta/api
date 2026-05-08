using Application.Administrativo.Cliente.Commands.CreateCliente;
using MediatR;

namespace Application.Administrativo.Cliente.Queries.GetClienteById;

public class GetClienteByIdQuery : IRequest<ClienteResponse?>
{
    public int Id { get; init; }
}
