using MediatR;

namespace Application.UseCases.OrdemServico.Queries.ListarOrdensServico;

public class ListarOrdensServicoQuery : IRequest<IReadOnlyList<OrdemServicoResponse>>;
