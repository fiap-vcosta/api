using Application.Abstractions.Events;
using Application.Abstractions.Gateways;
using Application.Abstractions.Services;
using Domain.Exceptions;
using Domain.OrdemServico.Events;
using MediatR;

namespace Application.UseCases.OrdemServico.Policies;

public class EnviarOrdemServicoParaAprovacaoPolicy(
    IOrdemServicoGateway ordemServicoGateway,
    IClienteGateway clienteGateway,
    ISMTPService smtpService
) : INotificationHandler<DomainEventNotification<DiagnosticoPreenchidoEvent>>
{
    public async Task Handle(DomainEventNotification<DiagnosticoPreenchidoEvent> notification, CancellationToken cancellationToken)
    {
        var idOrdemServico = notification.DomainEvent.IdOrdemServico;
        var ordemServico = await ordemServicoGateway.GetByIdAsync(idOrdemServico);
        if (ordemServico == null)
        {
            throw new DomainNotFoundException($"Ordem de Serviço com id {idOrdemServico} não encontrada");
        }
        
        var cliente = await clienteGateway.GetByIdAsync(ordemServico.Cliente.Id);
        if (cliente == null)
        {
            throw new DomainNotFoundException($"Cliente com id {ordemServico.Cliente.Id} não encontrado");
        }
        
        var conteudo = $"Ordem de Serviço {ordemServico.Id} disponível para aprovação!";
        await smtpService.EnviarEmail(cliente.Email, conteudo);
    }
}
