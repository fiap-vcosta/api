using Application.Abstractions.Events;
using Application.Abstractions.Gateways;
using Application.Abstractions.Services;
using Domain.Administrativo.Entities;
using Domain.Exceptions;
using Domain.OrdemServico.Events;
using MediatR;

namespace Application.UseCases.OrdemServico.Commands.EnviarOrdemServicoParaDiagnostico;

public class EnviarOrdemServicoParaDiagnosticoCommandHandler(
    IOrdemServicoGateway ordemServicoGateway,
    INotificacaoService notificacaoService,
    IMediator mediator
) : IRequestHandler<EnviarOrdemServicoParaDiagnosticoCommand>
{
    public async Task Handle(EnviarOrdemServicoParaDiagnosticoCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoGateway.GetByIdAsync(request.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new DomainNotFoundException($"Ordem de Serviço com id {request.IdOrdemServico} não encontrada.");
        }

        ordemServico.EnviarParaDiagnostico();

        await ordemServicoGateway.UpdateAsync(ordemServico);
        await notificacaoService.NotificarUsuariosPorTipo(TipoUsuario.Mecanico, $"Ordem de Serviço {ordemServico.Id} recebida para diagnóstico.");

        await mediator.Publish(new DomainEventNotification<OrdemServicoRecebidaDiagnosticoEvent>(new OrdemServicoRecebidaDiagnosticoEvent(ordemServico.Id)), cancellationToken);
    }
}
