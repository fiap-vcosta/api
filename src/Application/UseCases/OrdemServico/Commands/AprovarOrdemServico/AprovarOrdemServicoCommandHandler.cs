using Application.Abstractions.Events;
using Application.Abstractions.Gateways;
using Application.UseCases.OrdemServico.Responses;
using Domain.Exceptions;
using Domain.OrdemServico.Events;
using MediatR;

namespace Application.UseCases.OrdemServico.Commands.AprovarOrdemServico;

public class AprovarOrdemServicoCommandHandler(
    IOrdemServicoGateway ordemServicoGateway,
    IMediator mediator
) : IRequestHandler<AprovarOrdemServicoCommand, AprovarOrdemServicoCommandResponse>
{
    public async Task<AprovarOrdemServicoCommandResponse> Handle(AprovarOrdemServicoCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoGateway.GetByIdAsync(request.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new DomainNotFoundException($"Ordem de Serviço com id {request.IdOrdemServico} não encontrada");
        }
        
        ordemServico.AprovarServicosSugeridos();
        
        await ordemServicoGateway.UpdateAsync(ordemServico);
        await mediator.Publish(new DomainEventNotification<OrdemServicoAprovadaEvent>(new OrdemServicoAprovadaEvent(ordemServico.Id)), cancellationToken);

        return new AprovarOrdemServicoCommandResponse()
        {
            Id = ordemServico.Id,
            Status = ordemServico.Status,
            ValorTotal = ordemServico.ValorTotal,
            RecebidaEm = ordemServico.RecebidaEm,
            AprovadaEm = ordemServico.AprovadaEm ?? throw new BusinessRuleException("Data de aprovação precisa estar preenchida ao aprovar ordem de serviço"),
            Cliente = ClienteOrdemServicoResponse.From(ordemServico.Cliente),
            Veiculo = VeiculoOrdemServicoResponse.From(ordemServico.Veiculo),
            Servicos = ServicoOrdemServicoResponse.FromMany(ordemServico.Servicos)
        };
    }
}