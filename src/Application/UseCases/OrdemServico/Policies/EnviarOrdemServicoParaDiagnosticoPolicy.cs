using Application.Abstractions.Events;
using Application.UseCases.OrdemServico.Commands.EnviarOrdemServicoParaDiagnostico;
using Domain.OrdemServico.Events;
using MediatR;

namespace Application.UseCases.OrdemServico.Policies;

public class EnviarOrdemServicoParaDiagnosticoPolicy(IMediator mediator)
    : INotificationHandler<DomainEventNotification<OrdemServicoCriadaEvent>>
{
    public async Task Handle(DomainEventNotification<OrdemServicoCriadaEvent> notification, CancellationToken cancellationToken)
    {
        await mediator.Send(new EnviarOrdemServicoParaDiagnosticoCommand
        {
            IdOrdemServico = notification.DomainEvent.IdOrdemServico
        }, cancellationToken);
    }
}
