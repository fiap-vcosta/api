using Application.Core.OrdemServico.Commands.AlocarEstoqueOrdemServico;
using Domain.Estoque.Events;
using Domain.OrdemServico.Repositories;
using MediatR;

namespace Application.Core.OrdemServico.Policies;

public class ChecarFilaDeEsperaOrdemServicoPolicy(
    IOrdemServicoRepository ordemServicoRepository,
    IMediator mediator
) : INotificationHandler<ChegadaDeItensRegistradaEvent>
{
    public async Task Handle(ChegadaDeItensRegistradaEvent notification, CancellationToken cancellationToken)
    {
        var idItemAtualizado = notification.ItemEstoqueAggregateRoot.Id;
        
        var filaOrdemServico = await ordemServicoRepository.GetAguardandoPecaPorItemEstoqueAsync(idItemAtualizado);

        foreach (var ordemServico in filaOrdemServico)
        {
            await mediator.Send(new AlocarEstoqueOrdemServicoCommand(ordemServico.Id), cancellationToken);
        }
    }
}