namespace Api.Controllers.Cliente.UpdateCliente;

public class UpdateClienteRequest
{
    public string Nome { get; init; } = string.Empty;
    public int TipoDocumento { get; init; }
    public string Documento { get; init; } = string.Empty;
}
