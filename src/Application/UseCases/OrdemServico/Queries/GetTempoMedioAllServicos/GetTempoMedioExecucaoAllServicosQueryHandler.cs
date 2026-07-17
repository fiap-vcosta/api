using Application.Abstractions.Gateways;
using Application.UseCases.OrdemServico.Responses;
using MediatR;

namespace Application.UseCases.OrdemServico.Queries.GetTempoMedioAllServicos;

public class GetTempoMedioExecucaoAllServicosQueryHandler(IItemServicoGateway itemServicoGateway)
    : IRequestHandler<GetTempoMedioExecucaoAllServicosQuery, List<TempoMedioExecucaoResponse>>
{
    public async Task<List<TempoMedioExecucaoResponse>> Handle(GetTempoMedioExecucaoAllServicosQuery request, CancellationToken cancellationToken)
    {
        var temposMedios = await itemServicoGateway.GetAllTempoMedioExecucaoAsync();
        return temposMedios
            .Select(t => TempoMedioExecucaoResponse.From(t.idServico, t.totalExecucoes, t.execucaoMedia))
            .ToList();
    }
}