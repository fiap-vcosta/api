using Application.Abstractions.Events;
using Application.Abstractions.Services;
using Domain.OrdemServico.Events;
using MediatR;

namespace Application.UseCases.OrdemServico.Policies;

public class OrdemServicoMetricsPolicy(IOsMetrics metrics) :
    INotificationHandler<DomainEventNotification<OrdemServicoCriadaEvent>>,
    INotificationHandler<DomainEventNotification<OrdemServicoRecebidaDiagnosticoEvent>>,
    INotificationHandler<DomainEventNotification<DiagnosticoPreenchidoEvent>>,
    INotificationHandler<DomainEventNotification<OrdemServicoAprovadaEvent>>,
    INotificationHandler<DomainEventNotification<OrdemServicoRejeitadaEvent>>,
    INotificationHandler<DomainEventNotification<OrdemServicoDescartadaEvent>>
{
    public Task Handle(DomainEventNotification<OrdemServicoCriadaEvent> notification, CancellationToken cancellationToken)
    {
        metrics.IncrementCriada();
        metrics.IncrementEvento("criada");
        return Task.CompletedTask;
    }

    public Task Handle(DomainEventNotification<OrdemServicoRecebidaDiagnosticoEvent> notification, CancellationToken cancellationToken)
    {
        metrics.IncrementEvento("em_diagnostico");
        return Task.CompletedTask;
    }

    public Task Handle(DomainEventNotification<DiagnosticoPreenchidoEvent> notification, CancellationToken cancellationToken)
    {
        metrics.IncrementEvento("aguardando_aprovacao");
        return Task.CompletedTask;
    }

    public Task Handle(DomainEventNotification<OrdemServicoAprovadaEvent> notification, CancellationToken cancellationToken)
    {
        metrics.IncrementEvento("aprovada");
        return Task.CompletedTask;
    }

    public Task Handle(DomainEventNotification<OrdemServicoRejeitadaEvent> notification, CancellationToken cancellationToken)
    {
        metrics.IncrementEvento("rejeitada");
        return Task.CompletedTask;
    }

    public Task Handle(DomainEventNotification<OrdemServicoDescartadaEvent> notification, CancellationToken cancellationToken)
    {
        metrics.IncrementEvento("descartada");
        return Task.CompletedTask;
    }
}
