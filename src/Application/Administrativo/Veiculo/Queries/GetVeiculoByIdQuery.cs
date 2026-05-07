using Application.Administrativo.Veiculo.Commands;
using MediatR;

namespace Application.Administrativo.Veiculo.Queries;

public class GetVeiculoByIdQuery : IRequest<VeiculoResponse?>
{
    public int Id { get; init; }
}
