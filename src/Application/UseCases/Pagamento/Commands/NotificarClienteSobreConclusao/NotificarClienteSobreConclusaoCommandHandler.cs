using Application.Abstractions.Services;
using Domain.Exceptions;

using Application.Abstractions.Gateways;
using MediatR;

namespace Application.UseCases.Pagamento.Commands.NotificarClienteSobreConclusao;

public class NotificarClienteSobreConclusaoCommandHandler(IClienteGateway clienteGateway, ISmtpService smtpService)
    :IRequestHandler<NotificarClienteSobreConclusaoCommand, Unit>
{
    public async Task<Unit> Handle(NotificarClienteSobreConclusaoCommand request, CancellationToken cancellationToken)
    {
        var cliente = await clienteGateway.GetByIdAsync(request.IdCliente);
        if (cliente == null)
        {
            throw new DomainNotFoundException($"Item de estoque com id {request.IdCliente} não encontrado");
        }
        
        // TODO: Adicionar Ordem de Serviço
        
        var conteudo = $"Ordem de Serviço {request.IdOrdemServico} Finalizada!";
        await smtpService.EnviarEmail(cliente.Email, conteudo);

        return Unit.Value;
    }
}