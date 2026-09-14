using Application.Abstractions.Events;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Events;
using MediatR;

namespace Application.UseCases.OrdemServico;

public static class OrdemServicoStatusAlteradoPublisher
{
    public static async Task PublishAsync(
        IMediator mediator,
        OrdemServicoAggregateRoot ordemServico,
        CancellationToken cancellationToken)
    {
        foreach (var status in ordemServico.StatusAlterados)
        {
            await mediator.Publish(
                new DomainEventNotification<OrdemServicoStatusAlteradoEvent>(
                    new OrdemServicoStatusAlteradoEvent(ordemServico.Id, status)),
                cancellationToken);
        }

        ordemServico.LimparStatusAlterados();
    }
}
