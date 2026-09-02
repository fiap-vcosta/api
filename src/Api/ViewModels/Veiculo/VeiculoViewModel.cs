namespace Api.ViewModels.Veiculo;

public record VeiculoViewModel
{
    public required int Id { get; init; }
    public required int IdCliente { get; init; }
    public required string Placa { get; init; }
    public required string Modelo { get; init; }
    public required string Marca { get; init; }
}
