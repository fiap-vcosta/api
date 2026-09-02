using Application.UseCases.Administrativo.Veiculo.Responses;
using MediatR;

namespace Application.UseCases.Administrativo.Veiculo.Queries.GetVeiculosByCliente;

public class GetVeiculosByClienteQuery : IRequest<IEnumerable<VeiculoResponse>>
{
    public int IdCliente { get; init; }
}
