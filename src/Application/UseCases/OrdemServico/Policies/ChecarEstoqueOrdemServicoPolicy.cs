using Application.Abstractions.Events;
using Application.UseCases.OrdemServico.Commands.AlocarEstoqueOrdemServico;
using Domain.OrdemServico.Events;
using MediatR;

namespace Application.UseCases.OrdemServico.Policies;

public class ChecarEstoqueOrdemServicoPolicy(IMediator mediator) : INotificationHandler<DomainEventNotification<OrdemServicoAprovadaEvent>>
{
    public async Task Handle(DomainEventNotification<OrdemServicoAprovadaEvent> notification, CancellationToken cancellationToken)
    {
        await mediator.Send(new AlocarEstoqueOrdemServicoCommand(notification.DomainEvent.IdOrdemServico), cancellationToken);
    }
}
