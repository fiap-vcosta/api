using Application.Abstractions.Gateways;
using MediatR;

namespace Application.UseCases.Administrativo.Cliente.Queries.ConsultarClientePorDocumento;

public class ConsultarClientePorDocumentoQueryHandler(IClienteGateway clienteGateway)
    : IRequestHandler<ConsultarClientePorDocumentoQuery, bool>
{
    public async Task<bool> Handle(
        ConsultarClientePorDocumentoQuery request,
        CancellationToken cancellationToken)
    {
        var cliente = await clienteGateway.GetByDocumentoAsync(request.Documento);
        return cliente is not null;
    }
}
