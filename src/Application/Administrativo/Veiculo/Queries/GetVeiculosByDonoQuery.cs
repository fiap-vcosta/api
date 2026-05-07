using Application.Administrativo.Veiculo.Commands;
using MediatR;

namespace Application.Administrativo.Veiculo.Queries;

public class GetVeiculosByDonoQuery : IRequest<IEnumerable<VeiculoResponse>>
{
    public int DonoId { get; init; }
}
