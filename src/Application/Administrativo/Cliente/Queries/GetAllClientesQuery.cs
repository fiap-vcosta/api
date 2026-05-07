using Application.Administrativo.Cliente.Commands;
using MediatR;

namespace Application.Administrativo.Cliente.Queries;

public class GetAllClientesQuery : IRequest<IEnumerable<ClienteResponse>>;
