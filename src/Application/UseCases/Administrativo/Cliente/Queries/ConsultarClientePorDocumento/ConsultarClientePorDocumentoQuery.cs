using Application.UseCases.Administrativo.Cliente.Responses;
using MediatR;

namespace Application.UseCases.Administrativo.Cliente.Queries.ConsultarClientePorDocumento;

public class ConsultarClientePorDocumentoQuery : IRequest<ConsultarClientePorDocumentoResponse>
{
    public required string Documento { get; init; }
}
