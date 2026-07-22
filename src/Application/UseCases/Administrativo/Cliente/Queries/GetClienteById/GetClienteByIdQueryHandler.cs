using Application.UseCases.Administrativo.Cliente.Responses;
using Application.Abstractions.Gateways;
using MediatR;

namespace Application.UseCases.Administrativo.Cliente.Queries.GetClienteById;

public class GetClienteByIdQueryHandler(IClienteGateway clienteGateway)
    : IRequestHandler<GetClienteByIdQuery, ClienteResponse?>
{
    public async Task<ClienteResponse?> Handle(GetClienteByIdQuery request, CancellationToken cancellationToken)
    {
        var cliente = await clienteGateway.GetByIdAsync(request.Id);
        if (cliente == null)
        {
            return null;
        }

        return new ClienteResponse
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            TipoDocumento = cliente.TipoDocumento,
            Documento = cliente.Documento
        };
    }
}
