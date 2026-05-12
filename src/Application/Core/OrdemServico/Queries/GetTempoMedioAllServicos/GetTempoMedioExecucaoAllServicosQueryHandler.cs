using Domain.OrdemServico.Repositories;
using MediatR;

namespace Application.Core.OrdemServico.Queries.GetTempoMedioAllServicos;

public class GetTempoMedioExecucaoAllServicosQueryHandler(IItemServicoRepository itemServicoRepository)
    : IRequestHandler<GetTempoMedioExecucaoAllServicosQuery, List<IItemServicoRepository.TempoMedioExecucaoServico>>
{
    public async Task<List<IItemServicoRepository.TempoMedioExecucaoServico>> Handle(GetTempoMedioExecucaoAllServicosQuery request, CancellationToken cancellationToken)
    {
        return await itemServicoRepository.GetAllTempoMediaExecucaoAsync();
    }
}