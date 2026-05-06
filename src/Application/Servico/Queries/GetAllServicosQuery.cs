using Application.Servico.Commands;
using MediatR;

namespace Application.Servico.Queries;

public class GetAllServicosQuery : IRequest<IEnumerable<ServicoResponse>>
{
}
