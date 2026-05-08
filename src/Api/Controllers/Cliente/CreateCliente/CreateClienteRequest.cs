using Domain.Administrativo.Entities;

namespace Api.Controllers.Cliente.CreateCliente;

public class CreateClienteRequest
{
    public string Nome { get; init; } = string.Empty;
    public TipoDocumento TipoDocumento { get; init; }
    public string Documento { get; init; } = string.Empty;
}
