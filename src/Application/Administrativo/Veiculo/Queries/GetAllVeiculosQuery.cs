using Application.Administrativo.Veiculo.Commands;
using MediatR;

namespace Application.Administrativo.Veiculo.Queries;

public class GetAllVeiculosQuery : IRequest<IEnumerable<VeiculoResponse>>;
