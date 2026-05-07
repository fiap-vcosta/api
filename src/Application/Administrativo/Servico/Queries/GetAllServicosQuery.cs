using Application.Administrativo.Servico.Commands;
using MediatR;

namespace Application.Administrativo.Servico.Queries;

public class GetAllServicosQuery : IRequest<IEnumerable<ServicoResponse>>;
