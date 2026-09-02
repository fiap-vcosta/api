namespace Application.UseCases.Administrativo.Veiculo.Responses;

public class VeiculoResponse
{
    public int Id { get; init; }
    public int IdCliente { get; init; }
    public string Placa { get; init; } = string.Empty;
    public string Modelo { get; init; } = string.Empty;
    public string Marca { get; init; } = string.Empty;
}
