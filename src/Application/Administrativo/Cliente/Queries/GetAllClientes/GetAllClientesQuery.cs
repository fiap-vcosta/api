using Application.Administrativo.Cliente.Commands.CreateCliente;
using MediatR;

namespace Application.Administrativo.Cliente.Queries.GetAllClientes;

public class GetAllClientesQuery : IRequest<IEnumerable<ClienteResponse>>;
