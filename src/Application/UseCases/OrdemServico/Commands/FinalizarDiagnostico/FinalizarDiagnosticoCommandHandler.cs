using Application.Abstractions.Events;
using Application.Abstractions.Gateways;
using Application.UseCases.OrdemServico.Responses;
using Domain.Exceptions;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Events;
using MediatR;

namespace Application.UseCases.OrdemServico.Commands.FinalizarDiagnostico;

public class FinalizarDiagnosticoCommandHandler(
    IOrdemServicoGateway ordemServicoGateway,
    IMediator mediator
) : IRequestHandler<FinalizarDiagnosticoCommand, FinalizarDiagnosticoCommandResponse>
{
    public async Task<FinalizarDiagnosticoCommandResponse> Handle(FinalizarDiagnosticoCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoGateway.GetByIdAsync(request.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new DomainNotFoundException($"Ordem de Serviço com id {request.IdOrdemServico} não encontrada");
        }
        
        ordemServico.FinalizarDiagnostico();
        await ordemServicoGateway.UpdateAsync(ordemServico);

        switch (ordemServico.Status)
        {
            case StatusOrdemServico.AguardandoAprovacao:
                await mediator.Publish(new DomainEventNotification<DiagnosticoPreenchidoEvent>(new DiagnosticoPreenchidoEvent(ordemServico.Id)), cancellationToken);
                break;
            case StatusOrdemServico.Entregue:
                await mediator.Publish(new DomainEventNotification<OrdemServicoRejeitadaEvent>(new OrdemServicoRejeitadaEvent(ordemServico.Id)), cancellationToken);
                break;
            case StatusOrdemServico.ChecandoEstoque:
                await mediator.Publish(new DomainEventNotification<OrdemServicoAprovadaEvent>(new OrdemServicoAprovadaEvent(ordemServico.Id)), cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException($"Status {ordemServico.Status} inválido após finalizar diagnóstico da ordem de serviço {ordemServico.Id}.");
        }

        return new FinalizarDiagnosticoCommandResponse()
        {
            Id = ordemServico.Id,
            Status = ordemServico.Status,
            RecebidaEm = ordemServico.RecebidaEm,
            Cliente = ClienteOrdemServicoResponse.From(ordemServico.Cliente),
            Veiculo = VeiculoOrdemServicoResponse.From(ordemServico.Veiculo),
            Servicos = ServicoOrdemServicoResponse.FromMany(ordemServico.Servicos)
        };
    }
}