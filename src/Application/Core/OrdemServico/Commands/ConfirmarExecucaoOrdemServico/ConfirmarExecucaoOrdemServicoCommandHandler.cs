using System.Transactions;
using Application.Estoque.ItemEstoque.Commands.ConfirmarUtilizacaoItensEstoque;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Repositories;
using MediatR;

namespace Application.Core.OrdemServico.Commands.ConfirmarExecucaoOrdemServico;

public class ConfirmarExecucaoOrdemServicoCommandHandler(IOrdemServicoRepository ordemServicoRepository, IMediator mediator)
    : IRequestHandler<ConfirmarExecucaoOrdemServicoCommand, OrdemServicoAggregateRoot>
{
    public async Task<OrdemServicoAggregateRoot> Handle(ConfirmarExecucaoOrdemServicoCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoRepository.GetByIdAsync(request.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new KeyNotFoundException($"Ordem de Serviço com id {request.IdOrdemServico} não encontrada");
        }

        foreach (var servico in request.ServicoExecutados.Where(servico => ordemServico.Servicos.All(s => s.Id != servico.IdServico)))
        {
            throw new KeyNotFoundException($"Serviço com id {servico.IdServico} não pertence à Ordem de Serviço {request.IdOrdemServico}");
        }

        
        
        var opcoesTransacao = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
        using var scope = new TransactionScope(TransactionScopeOption.Required, opcoesTransacao, TransactionScopeAsyncFlowOption.Enabled);
        
        var command = new ConfirmarUtilizacaoItensEstoqueCommand { IdOrdemServico = ordemServico.Id };
        await mediator.Send(command, cancellationToken); 
        
        ordemServico.ConfirmarExecucao(request.ServicoExecutados);
        await ordemServicoRepository.UpdateAsync(ordemServico);
        
        scope.Complete();
        return ordemServico;
    }
}