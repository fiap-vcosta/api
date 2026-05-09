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
        await mediator.Publish(new DiagnosticoPreenchidoEvent(ordemServico.Id), cancellationToken);

        return new FinalizarDiagnosticoCommandResponse()
        {
            Id = ordemServico.Id,
            Status = ordemServico.Status,
            RecebidaEm = ordemServico.RecebidaEm,
            Cliente = ordemServico.Cliente,
            Veiculo = ordemServico.Veiculo,
            Itens = ordemServico.ItensOrdemServico.ToList()
        };
    }
}