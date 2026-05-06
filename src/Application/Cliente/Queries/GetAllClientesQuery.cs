using Application.Cliente.Commands;
using MediatR;

namespace Application.Cliente.Queries;

public class GetAllClientesQuery : IRequest<IEnumerable<ClienteResponse>>
{
}
