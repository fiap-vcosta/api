using Application.Abstractions.Events;
using Application.Abstractions.Gateways;
using Application.Abstractions.Services;
using Domain.Administrativo.Entities;
using Domain.Exceptions;
using Domain.OrdemServico.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.OrdemServico.Commands.EnviarOrdemServicoParaDiagnostico;

public class EnviarOrdemServicoParaDiagnosticoCommandHandler(
    IOrdemServicoGateway ordemServicoGateway,
    INotificacaoService notificacaoService,
    IMediator mediator,
    ILogger<EnviarOrdemServicoParaDiagnosticoCommandHandler> logger
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
        OrdemServicoStatusLog.Emit(logger, ordemServico.Id, ordemServico.Status);
        await notificacaoService.NotificarUsuariosPorTipo(TipoUsuario.Mecanico, $"Ordem de Serviço {ordemServico.Id} recebida para diagnóstico.");

        await mediator.Publish(new DomainEventNotification<OrdemServicoRecebidaDiagnosticoEvent>(new OrdemServicoRecebidaDiagnosticoEvent(ordemServico.Id)), cancellationToken);
    }
}
