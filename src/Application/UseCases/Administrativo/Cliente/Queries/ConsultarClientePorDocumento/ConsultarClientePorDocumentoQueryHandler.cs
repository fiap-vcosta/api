using Application.Abstractions.Gateways;
using Application.UseCases.Administrativo.Cliente.Responses;
using MediatR;

namespace Application.UseCases.Administrativo.Cliente.Queries.ConsultarClientePorDocumento;

public class ConsultarClientePorDocumentoQueryHandler(IClienteGateway clienteGateway)
    : IRequestHandler<ConsultarClientePorDocumentoQuery, ConsultarClientePorDocumentoResponse>
{
    public async Task<ConsultarClientePorDocumentoResponse> Handle(
        ConsultarClientePorDocumentoQuery request,
        CancellationToken cancellationToken)
    {
        var documento = NormalizeDocumento(request.Documento);
        var cliente = await clienteGateway.GetByDocumentoAsync(documento);

        if (cliente is null)
        {
            return new ConsultarClientePorDocumentoResponse { Existe = false };
        }

        return new ConsultarClientePorDocumentoResponse
        {
            Existe = true,
            Documento = cliente.Documento,
            TipoDocumento = cliente.TipoDocumento
        };
    }

    private static string NormalizeDocumento(string documento) =>
        new(documento.Where(char.IsDigit).ToArray());
}
