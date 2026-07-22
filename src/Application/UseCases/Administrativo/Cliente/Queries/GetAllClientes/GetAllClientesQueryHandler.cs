using Application.UseCases.Administrativo.Cliente.Responses;
using Application.Abstractions.Gateways;
using MediatR;

namespace Application.UseCases.Administrativo.Cliente.Queries.GetAllClientes;

public class GetAllClientesQueryHandler(IClienteGateway clienteGateway)
    : IRequestHandler<GetAllClientesQuery, IEnumerable<ClienteResponse>>
{
    public async Task<IEnumerable<ClienteResponse>> Handle(GetAllClientesQuery request, CancellationToken cancellationToken)
    {
        var clientes = await clienteGateway.GetAllAsync();
        return clientes.Select(c => new ClienteResponse
        {
            Id = c.Id,
            Nome = c.Nome,
            TipoDocumento = c.TipoDocumento,
            Documento = c.Documento
        }).ToList();
    }
}
