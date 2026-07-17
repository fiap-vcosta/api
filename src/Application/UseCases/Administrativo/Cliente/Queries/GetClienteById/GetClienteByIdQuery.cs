using Application.UseCases.Administrativo.Cliente.Responses;
using MediatR;

namespace Application.UseCases.Administrativo.Cliente.Queries.GetClienteById;

public class GetClienteByIdQuery : IRequest<ClienteResponse?>
{
    public int Id { get; init; }
}
