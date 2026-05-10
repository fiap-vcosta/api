using Application.Abstractions.Services;
using Domain.Administrativo.Repositories;
using Domain.OrdemServico.Events;
using Domain.OrdemServico.Repositories;
using MediatR;

namespace Application.Core.OrdemServico.Policies;

public class EnviarOrdemServicoParaAprovacaoPolicy(
    IOrdemServicoRepository ordemServicoRepository,
    IClienteRepository clienteRepository,
    ISMTPService smtpService
) : INotificationHandler<DiagnosticoPreenchidoEvent>
{
    public async Task Handle(DiagnosticoPreenchidoEvent notification, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoRepository.GetByIdAsync(notification.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new KeyNotFoundException($"Ordem de Serviço com id {notification.IdOrdemServico} não encontrada");
        }
        
        var cliente = await clienteRepository.GetByIdAsync(ordemServico.Cliente.Id);
        if (cliente == null)
        {
            throw new KeyNotFoundException($"Cliente com id {ordemServico.Cliente.Id} não encontrado");
        }
        
        var conteudo = $"Ordem de Serviço {ordemServico.Id} disponível para aprovação!";
        await smtpService.EnviarEmail(cliente.Email, conteudo);
    }
}