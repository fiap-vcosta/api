namespace Api.Controllers.Veiculo.UpdateVeiculo;

public class UpdateVeiculoRequest
{
    public string Placa { get; init; } = string.Empty;
    public int DonoId { get; init; }
    public string Modelo { get; init; } = string.Empty;
    public string Marca { get; init; } = string.Empty;
}
