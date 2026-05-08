using Application.Administrativo.Servico.Commands.CreateServico;
using MediatR;

namespace Application.Administrativo.Servico.Queries.GetAllServicos;

public class GetAllServicosQuery : IRequest<IEnumerable<ServicoResponse>>;
