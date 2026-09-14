using Application.Abstractions.Events;
using Application.Abstractions.Gateways;
using Application.UseCases.OrdemServico.Responses;
using Domain.Exceptions;
using Domain.OrdemServico.Events;
using MediatR;

namespace Application.UseCases.OrdemServico.Commands.DescartarOrdemServico;

public class DescartarOrdemServicoCommandHandler(
    IOrdemServicoGateway ordemServicoGateway,
    IMediator mediator
) : IRequestHandler<DescartarOrdemServicoCommand, DescartarOrdemServicoResponse>
{
    public async Task<DescartarOrdemServicoResponse> Handle(DescartarOrdemServicoCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoGateway.GetByIdAsync(request.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new DomainNotFoundException($"Ordem de Serviço com id {request.IdOrdemServico} não encontrada");
        }
        
        ordemServico.Descartar();
        
        await ordemServicoGateway.UpdateAsync(ordemServico);
        await mediator.Publish(new DomainEventNotification<OrdemServicoDescartadaEvent>(new OrdemServicoDescartadaEvent(ordemServico.Id)), cancellationToken);

        return new DescartarOrdemServicoResponse
        {
            Id = ordemServico.Id,
            Status = ordemServico.Status,
            RecebidaEm = ordemServico.RecebidaEm,
            DescartadaEm = ordemServico.DescartadaEm ?? throw new BusinessRuleException("Data de descarte precisa estar preenchida ao descartar ordem de serviço"),
            Cliente = ClienteOrdemServicoResponse.From(ordemServico.Cliente),
            Veiculo = VeiculoOrdemServicoResponse.From(ordemServico.Veiculo),
            Itens = ServicoOrdemServicoResponse.FromMany(ordemServico.Servicos)
        };
    }
}