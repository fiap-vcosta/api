using Application.Veiculo.Commands;
using MediatR;

namespace Application.Veiculo.Queries;

public class GetVeiculoByIdQuery : IRequest<VeiculoResponse?>
{
    public int Id { get; init; }
}
