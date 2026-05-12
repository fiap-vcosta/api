using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Repositories;
using MediatR;

namespace Application.Core.OrdemServico.Commands.ConfirmarPagamentoOrdemServico;

public class ConfirmarPagamentoOrdemServicoCommandHandler(IOrdemServicoRepository ordemServicoRepository)
    : IRequestHandler<ConfirmarPagamentoOrdemServicoCommand, OrdemServicoAggregateRoot>
{
    public async Task<OrdemServicoAggregateRoot> Handle(ConfirmarPagamentoOrdemServicoCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoRepository.GetByIdAsync(request.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new KeyNotFoundException($"Ordem de Serviço com id {request.IdOrdemServico} não encontrada");
        }
        
        ordemServico.ConfirmarPagamento();
        await ordemServicoRepository.UpdateAsync(ordemServico);

        return ordemServico;
    }
}