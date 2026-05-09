using Domain.OrdemServico.Events;
using Domain.OrdemServico.Repositories;
using MediatR;

namespace Application.Core.OrdemServico.Policies;

public class EnviaOrdemServicoParaDiagnosticoPolicy(
    IOrdemServicoRepository ordemServicoRepository
) : INotificationHandler<OrdemServicoCriadaEvent>
{
    public async Task Handle(OrdemServicoCriadaEvent notification, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoRepository.GetByIdAsync(notification.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new KeyNotFoundException($"Ordem de Serviço com id {notification.IdOrdemServico} não encontrada");
        }

        ordemServico.EnviarParaDiagnostico();

        await ordemServicoRepository.UpdateAsync(ordemServico);
    }
}