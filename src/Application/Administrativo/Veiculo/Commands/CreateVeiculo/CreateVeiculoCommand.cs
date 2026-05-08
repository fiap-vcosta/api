using MediatR;

namespace Application.Administrativo.Veiculo.Commands.CreateVeiculo;

public class CreateVeiculoCommand : IRequest<VeiculoResponse>
{
    public string Placa { get; init; } = string.Empty;
    public int DonoId { get; init; }
    public string Modelo { get; init; } = string.Empty;
    public string Marca { get; init; } = string.Empty;
}

public class VeiculoResponse
{
    public int Id { get; init; }
    public string Placa { get; init; } = string.Empty;
    public int DonoId { get; init; }
    public string Modelo { get; init; } = string.Empty;
    public string Marca { get; init; } = string.Empty;
}
