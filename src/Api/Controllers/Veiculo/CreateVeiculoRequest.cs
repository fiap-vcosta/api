namespace Api.Controllers.Veiculo;

public class CreateVeiculoRequest
{
    public string Placa { get; set; } = string.Empty;
    public int DonoId { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
}
