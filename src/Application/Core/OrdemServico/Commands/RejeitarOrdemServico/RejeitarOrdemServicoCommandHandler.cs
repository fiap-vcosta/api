using Domain.OrdemServico.Repositories;
using MediatR;

namespace Application.Core.OrdemServico.Commands.RejeitarOrdemServico;

public class RejeitarOrdemServicoCommandHandler(
    IOrdemServicoRepository ordemServicoRepository
) : IRequestHandler<RejeitarOrdemServicoCommand, RejeitarOrdemServicoCommandResponse>
{
    public async Task<RejeitarOrdemServicoCommandResponse> Handle(RejeitarOrdemServicoCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoRepository.GetByIdAsync(request.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new KeyNotFoundException($"Ordem de Serviço com id {request.IdOrdemServico} não encontrada");
        }
        
        ordemServico.RejeitarServicosSugeridos();
        
        await ordemServicoRepository.UpdateAsync(ordemServico);

        return new RejeitarOrdemServicoCommandResponse()
        {
            Id = ordemServico.Id,
            Status = ordemServico.Status,
            ValorTotal = ordemServico.ValorTotal,
            RecebidaEm = ordemServico.RecebidaEm,
            EntregueEm = ordemServico.EntregueEm ?? throw new InvalidOperationException("Data de entrega precisa estar preenchida ao rejeitar ordem de serviço"),
            Cliente = ordemServico.Cliente,
            Veiculo = ordemServico.Veiculo,
            Servicos = ordemServico.ItensOrdemServico.ToList()
        };
    }
}