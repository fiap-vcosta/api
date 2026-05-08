using Application.Abstractions.Services;
using Domain.Administrativo.Repositories;
using MediatR;

namespace Application.Pagamento.Commands.NotificarClienteSobreConclusao;

public class NotificarClienteSobreConclusaoCommandHandler(IClienteRepository clienteRepository, ISMTPService smtpService)
    :IRequestHandler<NotificarClienteSobreConclusaoCommand, Unit>
{
    public async Task<Unit> Handle(NotificarClienteSobreConclusaoCommand request, CancellationToken cancellationToken)
    {
        var cliente = await clienteRepository.GetByIdAsync(request.IdCliente);
        if (cliente == null)
        {
            throw new KeyNotFoundException($"Item de estoque com id {request.IdCliente} não encontrado");
        }
        
        // TODO: Adicionar Ordem de Serviço
        
        var conteudo = $"Ordem de Serviço {request.IdOrdemServico} Finalizada!";
        await smtpService.EnviarEmail(cliente.Email, conteudo);

        return Unit.Value;
    }
}