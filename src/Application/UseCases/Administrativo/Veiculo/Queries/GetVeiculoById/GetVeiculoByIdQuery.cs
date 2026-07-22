using Application.UseCases.Administrativo.Veiculo.Responses;
using MediatR;

namespace Application.UseCases.Administrativo.Veiculo.Queries.GetVeiculoById;

public class GetVeiculoByIdQuery : IRequest<VeiculoResponse?>
{
    public int Id { get; init; }
}
