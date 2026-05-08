using Application.Administrativo.Veiculo.Commands.CreateVeiculo;
using MediatR;

namespace Application.Administrativo.Veiculo.Queries.GetAllVeiculos;

public class GetAllVeiculosQuery : IRequest<IEnumerable<VeiculoResponse>>;
