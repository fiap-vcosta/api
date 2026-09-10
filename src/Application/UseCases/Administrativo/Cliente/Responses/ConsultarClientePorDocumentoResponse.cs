using Domain.Administrativo.Entities;

namespace Application.UseCases.Administrativo.Cliente.Responses;

public class ConsultarClientePorDocumentoResponse
{
    public bool Existe { get; init; }
    public string? Documento { get; init; }
    public TipoDocumento? TipoDocumento { get; init; }
}
