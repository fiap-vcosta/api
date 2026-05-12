using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Events;
using Domain.OrdemServico.Repositories;
using MediatR;

namespace Application.Core.OrdemServico.Commands.FinalizarDiagnostico;

public class FinalizarDiagnosticoCommandHandler(
    IOrdemServicoRepository ordemServicoRepository,
    IMediator mediator
) : IRequestHandler<FinalizarDiagnosticoCommand, FinalizarDiagnosticoCommandResponse>
{
    public async Task<FinalizarDiagnosticoCommandResponse> Handle(FinalizarDiagnosticoCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoRepository.GetByIdAsync(request.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new KeyNotFoundException($"Ordem de Serviço com id {request.IdOrdemServico} não encontrada");
        }
        
        ordemServico.FinalizarDiagnostico();
        await ordemServicoRepository.UpdateAsync(ordemServico);

        switch (ordemServico.Status)
        {
            case StatusOrdemServico.AguardandoAprovacao:
                await mediator.Publish(new DiagnosticoPreenchidoEvent(ordemServico.Id), cancellationToken);
                break;
            case StatusOrdemServico.Entregue:
                await mediator.Publish(new OrdemServicoRejeitadaEvent(ordemServico.Id), cancellationToken);
                break;
            case StatusOrdemServico.ChecandoEstoque:
                await mediator.Publish(new OrdemServicoAprovadaEvent(ordemServico.Id), cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException($"Status {ordemServico.Status} inválido após finalizar disgnóstico da ordem de serviço {ordemServico.Id}.");
        }

        return new FinalizarDiagnosticoCommandResponse()
        {
            Id = ordemServico.Id,
            Status = ordemServico.Status,
            RecebidaEm = ordemServico.RecebidaEm,
            Cliente = ordemServico.Cliente,
            Veiculo = ordemServico.Veiculo,
            Servicos = ordemServico.Servicos.ToList()
        };
    }
}