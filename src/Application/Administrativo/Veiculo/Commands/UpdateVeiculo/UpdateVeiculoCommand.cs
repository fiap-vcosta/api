using Application.Administrativo.Veiculo.Commands.CreateVeiculo;
using MediatR;

namespace Application.Administrativo.Veiculo.Commands.UpdateVeiculo;

public class UpdateVeiculoCommand : IRequest<VeiculoResponse>
{
    public int Id { get; init; }
    public string Placa { get; init; } = string.Empty;
    public int DonoId { get; init; }
    public string Modelo { get; init; } = string.Empty;
    public string Marca { get; init; } = string.Empty;
}
