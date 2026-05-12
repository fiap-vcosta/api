using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Repositories;
using MediatR;

namespace Application.Core.OrdemServico.Queries.GetOrdemServicoById;

public class GetOrdemServicoByIdQueryHandler(IOrdemServicoRepository ordemServicoRepository) : IRequestHandler<GetOrdemServicoByIdQuery, OrdemServicoAggregateRoot?>
{
    public async Task<OrdemServicoAggregateRoot?> Handle(GetOrdemServicoByIdQuery request, CancellationToken cancellationToken)
    {
        return await ordemServicoRepository.GetByIdAsync(request.Id);
    }
}