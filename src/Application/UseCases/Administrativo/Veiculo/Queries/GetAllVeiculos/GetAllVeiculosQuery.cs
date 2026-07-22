using Application.UseCases.Administrativo.Veiculo.Responses;
using MediatR;

namespace Application.UseCases.Administrativo.Veiculo.Queries.GetAllVeiculos;

public class GetAllVeiculosQuery : IRequest<IEnumerable<VeiculoResponse>>;
