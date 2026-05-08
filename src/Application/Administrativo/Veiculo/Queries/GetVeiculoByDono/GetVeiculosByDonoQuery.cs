using Application.Administrativo.Veiculo.Commands.CreateVeiculo;
using MediatR;

namespace Application.Administrativo.Veiculo.Queries.GetVeiculoByDono;

public class GetVeiculosByDonoQuery : IRequest<IEnumerable<VeiculoResponse>>
{
    public int DonoId { get; init; }
}
