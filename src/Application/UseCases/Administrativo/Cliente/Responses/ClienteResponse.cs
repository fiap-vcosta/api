using Domain.Administrativo.Entities;

namespace Application.UseCases.Administrativo.Cliente.Responses;

public class ClienteResponse
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public TipoDocumento TipoDocumento { get; init; }
    public string Documento { get; init; } = string.Empty;
}
