using Application.Veiculo.Commands;
using MediatR;

namespace Application.Veiculo.Queries;

public class GetAllVeiculosQuery : IRequest<IEnumerable<VeiculoResponse>>
{
}
