namespace Api.Controllers.Cliente;

public class CreateClienteRequest
{
    public string Nome { get; set; } = string.Empty;
    public int TipoDocumento { get; set; }
    public string Documento { get; set; } = string.Empty;
}
