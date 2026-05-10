using Domain.OrdemServico.Events;
using Domain.OrdemServico.Repositories;
using MediatR;

namespace Application.Core.OrdemServico.Commands.AprovarOrdemServico;

public class AprovarOrdemServicoCommandHandler(
    IOrdemServicoRepository ordemServicoRepository,
    IMediator mediator
) : IRequestHandler<AprovarOrdemServicoCommand, AprovarOrdemServicoCommandResponse>
{
    public async Task<AprovarOrdemServicoCommandResponse> Handle(AprovarOrdemServicoCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoRepository.GetByIdAsync(request.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new KeyNotFoundException($"Ordem de Serviço com id {request.IdOrdemServico} não encontrada");
        }
        
        ordemServico.AprovarServicosSugeridos();
        
        await ordemServicoRepository.UpdateAsync(ordemServico);
        await mediator.Publish(new OrdemServicoAprovadaEvent(ordemServico.Id), cancellationToken);

        return new AprovarOrdemServicoCommandResponse()
        {
            Id = ordemServico.Id,
            Status = ordemServico.Status,
            ValorTotal = ordemServico.ValorTotal,
            RecebidaEm = ordemServico.RecebidaEm,
            AprovadaEm = ordemServico.AprovadaEm ?? throw new InvalidOperationException("Data de aprovação precisa estar preenchida ao aprovar ordem de serviço"),
            Cliente = ordemServico.Cliente,
            Veiculo = ordemServico.Veiculo,
            Servicos = ordemServico.ItensOrdemServico.ToList()
        };
    }
}