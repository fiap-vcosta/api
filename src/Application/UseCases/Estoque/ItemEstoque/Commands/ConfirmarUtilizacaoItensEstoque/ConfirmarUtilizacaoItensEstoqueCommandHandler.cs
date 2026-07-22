using Application.Abstractions.Gateways;
using Domain.Exceptions;
using MediatR;

namespace Application.UseCases.Estoque.ItemEstoque.Commands.ConfirmarUtilizacaoItensEstoque;

public class ConfirmarUtilizacaoItensEstoqueCommandHandler(IOrdemServicoGateway ordemServicoGateway, IItemEstoqueGateway itemEstoqueGateway)
    : IRequestHandler<ConfirmarUtilizacaoItensEstoqueCommand, Unit>
{
    public async Task<Unit> Handle(ConfirmarUtilizacaoItensEstoqueCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoGateway.GetByIdAsync(request.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new DomainNotFoundException($"Ordem de Serviço com id {request.IdOrdemServico} não encontrada");
        }
        
        var itensEstoque = await itemEstoqueGateway.GetUtilizadosByOrdemServico(request.IdOrdemServico);
        foreach (var item in itensEstoque)
        {
            var quantidade = ordemServico.ItensNecessariosParaExecucao
                .Where(i => i.ItemEstoque.Id == item.Id)
                .Sum(i => i.Quantidade);
                
            item.ConfirmarUtilizacao(quantidade);
            await itemEstoqueGateway.UpdateAsync(item);
        }
        
        return Unit.Value;
    }
}