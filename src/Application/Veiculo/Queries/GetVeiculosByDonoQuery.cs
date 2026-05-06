using Application.Veiculo.Commands;
using MediatR;

namespace Application.Veiculo.Queries;

public class GetVeiculosByDonoQuery : IRequest<IEnumerable<VeiculoResponse>>
{
    public int DonoId { get; init; }
}
