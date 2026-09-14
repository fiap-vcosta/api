using Application.Abstractions.Events;
using Application.Abstractions.Services;
using Domain.OrdemServico.Events;
using MediatR;

namespace Application.UseCases.OrdemServico.Policies;

public class OrdemServicoMetricsPolicy(IOsMetrics metrics)
    : INotificationHandler<DomainEventNotification<OrdemServicoStatusAlteradoEvent>>
{
    public Task Handle(
        DomainEventNotification<OrdemServicoStatusAlteradoEvent> notification,
        CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        metrics.IncrementStatus(domainEvent.IdOrdemServico, domainEvent.Status);
        return Task.CompletedTask;
    }
}
