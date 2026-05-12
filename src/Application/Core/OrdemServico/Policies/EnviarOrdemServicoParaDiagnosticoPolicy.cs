using Application.Abstractions.Services;
using Domain.Administrativo.Entities;
using Domain.OrdemServico.Events;
using Domain.OrdemServico.Repositories;
using MediatR;

namespace Application.Core.OrdemServico.Policies;

public class EnviarOrdemServicoParaDiagnosticoPolicy(
    IOrdemServicoRepository ordemServicoRepository,
    INotificacaoService notificacaoService,
    IMediator mediator
) : INotificationHandler<OrdemServicoCriadaEvent>
{
    public async Task Handle(OrdemServicoCriadaEvent notification, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoRepository.GetByIdAsync(notification.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new KeyNotFoundException($"Ordem de Serviço com id {notification.IdOrdemServico} não encontrada.");
        }

        ordemServico.EnviarParaDiagnostico();
        
        await ordemServicoRepository.UpdateAsync(ordemServico);
        await notificacaoService.NotificarUsuariosPorTipo(TipoUsuario.Mecanico, $"Ordem de Serviço {ordemServico.Id} recebida para diagnóstico.");
        
        await mediator.Publish(new OrdemServicoRecebidaDiagnosticoEvent(ordemServico.Id), cancellationToken);
    }
}