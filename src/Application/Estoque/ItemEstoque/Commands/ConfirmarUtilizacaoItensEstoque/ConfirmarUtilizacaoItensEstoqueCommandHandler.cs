using Domain.Estoque.Repositories;
using Domain.OrdemServico.Repositories;
using MediatR;

namespace Application.Estoque.ItemEstoque.Commands.ConfirmarUtilizacaoItensEstoque;

public class ConfirmarUtilizacaoItensEstoqueCommandHandler(IOrdemServicoRepository ordemServicoRepository, IItemEstoqueRepository itemEstoqueRepository)
    : IRequestHandler<ConfirmarUtilizacaoItensEstoqueCommand, Unit>
{
    public async Task<Unit> Handle(ConfirmarUtilizacaoItensEstoqueCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoRepository.GetByIdAsync(request.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new KeyNotFoundException($"Ordem de Serviço com id {request.IdOrdemServico} não encontrada");
        }
        
        var itensEstoque = await itemEstoqueRepository.GetUtilizadosByOrdemServico(request.IdOrdemServico);
        foreach (var item in itensEstoque)
        {
            var quantidade = ordemServico.ItensNecessariosParaExecucao
                .Where(i => i.ItemEstoque.Id == item.Id)
                .Sum(i => i.Quantidade);
                
            item.ConfirmarUtilizacao(quantidade);
            await itemEstoqueRepository.UpdateAsync(item);
        }
        
        return Unit.Value;
    }
}