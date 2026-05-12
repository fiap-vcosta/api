using Domain.OrdemServico.Events;
using Domain.OrdemServico.Repositories;
using MediatR;

namespace Application.Core.OrdemServico.Commands.DescartarOrdemServico;

public class DescartarOrdemServicoCommandHandler(
    IOrdemServicoRepository ordemServicoRepository,
    IMediator mediator
) : IRequestHandler<DescartarOrdemServicoCommand, DescartarOrdemServicoResponse>
{
    public async Task<DescartarOrdemServicoResponse> Handle(DescartarOrdemServicoCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoRepository.GetByIdAsync(request.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new KeyNotFoundException($"Ordem de Serviço com id {request.IdOrdemServico} não encontrada");
        }
        
        ordemServico.Descartar();
        
        await ordemServicoRepository.UpdateAsync(ordemServico);
        await mediator.Publish(new OrdemServicoDescartadaEvent(ordemServico.Id), cancellationToken);

        return new DescartarOrdemServicoResponse
        {
            Id = ordemServico.Id,
            Status = ordemServico.Status,
            RecebidaEm = ordemServico.RecebidaEm,
            DescartadaEm = ordemServico.DescartadaEm ?? throw new InvalidOperationException("Data de descarte precisa estar preenchida ao descartas ordem de serviço"),
            Cliente = ordemServico.Cliente,
            Veiculo = ordemServico.Veiculo,
            Itens = ordemServico.Servicos.ToList()
        };
    }
}