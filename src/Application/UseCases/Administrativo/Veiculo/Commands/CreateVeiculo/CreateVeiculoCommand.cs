using Application.UseCases.Administrativo.Veiculo.Responses;
using MediatR;

namespace Application.UseCases.Administrativo.Veiculo.Commands.CreateVeiculo;

public class CreateVeiculoCommand : IRequest<VeiculoResponse>
{
    public string Placa { get; init; } = string.Empty;
    public int IdCliente { get; init; }
    public string Modelo { get; init; } = string.Empty;
    public string Marca { get; init; } = string.Empty;
}
