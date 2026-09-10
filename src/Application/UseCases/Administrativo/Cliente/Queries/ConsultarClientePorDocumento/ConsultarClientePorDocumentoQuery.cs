using MediatR;

namespace Application.UseCases.Administrativo.Cliente.Queries.ConsultarClientePorDocumento;

public class ConsultarClientePorDocumentoQuery : IRequest<bool>
{
    public required string Documento { get; init; }
}
