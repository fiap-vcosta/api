using Domain.Estoque.Events;
using MediatR;

namespace Application.Estoque.ItemEstoque.Policies;

public class TravaItensNecessariosPolicy : INotificationHandler<ChegadaDeItensRegistradaEvent>
{
    public Task Handle(ChegadaDeItensRegistradaEvent notification, CancellationToken cancellationToken)
    {
        // TODO: Implementar após repositório da OS
        return Task.CompletedTask;
    }
}
