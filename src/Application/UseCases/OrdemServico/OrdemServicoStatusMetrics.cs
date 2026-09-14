using Application.Abstractions.Services;
using Domain.OrdemServico.Entities;

namespace Application.UseCases.OrdemServico;

public static class OrdemServicoStatusMetrics
{
    public static void EmitIfChanged(
        IOsMetrics metrics,
        OrdemServicoAggregateRoot ordemServico,
        StatusOrdemServico statusAnterior)
    {
        if (statusAnterior == ordemServico.Status)
        {
            return;
        }

        metrics.IncrementStatus(ordemServico.Id, ordemServico.Status);
    }
}
