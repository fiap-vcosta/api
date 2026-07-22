using Application.UseCases.Administrativo.Servico.Responses;
using MediatR;

namespace Application.UseCases.Administrativo.Servico.Queries.GetAllServicos;

public class GetAllServicosQuery : IRequest<IEnumerable<ServicoResponse>>;
