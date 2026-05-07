namespace Api.Controllers.Cliente.CreateCliente;

public class CreateClienteRequest
{
    public string Nome { get; init; } = string.Empty;
    public int TipoDocumento { get; init; }
    public string Documento { get; init; } = string.Empty;
}
