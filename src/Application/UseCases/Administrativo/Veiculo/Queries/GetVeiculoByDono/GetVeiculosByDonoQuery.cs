using Application.UseCases.Administrativo.Veiculo.Responses;
using MediatR;

namespace Application.UseCases.Administrativo.Veiculo.Queries.GetVeiculoByDono;

public class GetVeiculosByDonoQuery : IRequest<IEnumerable<VeiculoResponse>>
{
    public int IdDono { get; init; }
}
