using Application.UseCases.Administrativo.Cliente.Responses;
using MediatR;

namespace Application.UseCases.Administrativo.Cliente.Queries.GetAllClientes;

public class GetAllClientesQuery : IRequest<IEnumerable<ClienteResponse>>;
