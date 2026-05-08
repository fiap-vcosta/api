using Application.Administrativo.Veiculo.Commands.CreateVeiculo;
using MediatR;

namespace Application.Administrativo.Veiculo.Queries.GetVeiculoById;

public class GetVeiculoByIdQuery : IRequest<VeiculoResponse?>
{
    public int Id { get; init; }
}
