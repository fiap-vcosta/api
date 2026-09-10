using Domain.Administrativo.Entities;

namespace Api.ViewModels.Cliente;

public record ConsultarClientePorDocumentoViewModel
{
    public required bool Existe { get; init; }
    public string? Documento { get; init; }
    public TipoDocumento? TipoDocumento { get; init; }
}
