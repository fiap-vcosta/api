using Application.Core.OrdemServico.Commands.AlocarEstoqueOrdemServico;
using Domain.OrdemServico.Events;
using MediatR;

namespace Application.Core.OrdemServico.Policies;

public class ChecarEstoqueOrdemServicoPolicy(IMediator mediator ) : INotificationHandler<OrdemServicoAprovadaEvent>
{
    public async Task Handle(OrdemServicoAprovadaEvent notification, CancellationToken cancellationToken)
    {
        await mediator.Send(new AlocarEstoqueOrdemServicoCommand(notification.IdOrdemServico), cancellationToken);
    }
}