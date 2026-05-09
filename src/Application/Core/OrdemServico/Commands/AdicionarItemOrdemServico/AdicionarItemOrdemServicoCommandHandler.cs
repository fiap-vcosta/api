using Domain.Administrativo.Repositories;
using Domain.Estoque.Repositories;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Repositories;
using MediatR;

namespace Application.Core.OrdemServico.Commands.AdicionarItemOrdemServico;

public class AdicionarItemOrdemServicoCommandHandler(
    IOrdemServicoRepository ordemServicoRepository,
    IServicoRepository servicoRepository,
    IItemEstoqueRepository  itemEstoqueRepository
) : IRequestHandler<AdicionarItemOrdemServicoCommand, AdicionarItemOrdemServicoCommandResponse>
{
    public async Task<AdicionarItemOrdemServicoCommandResponse> Handle(AdicionarItemOrdemServicoCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoRepository.GetByIdAsync(request.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new KeyNotFoundException($"Ordem de Serviço com id {request.IdOrdemServico} não encontrada");
        }
        
        var servico = await servicoRepository.GetByIdAsync(request.IdServico);
        if (servico == null)
        {
            throw new KeyNotFoundException($"Serviço com id {request.IdOrdemServico} não encontrado");
        }

        var itensNecessarios = new List<ItemEstoqueOrdemServico.ItemNecessario>();
        foreach (var itemNecessario in request.ItensNecessarios)
        {
            var itemEstoque = await itemEstoqueRepository.GetByIdAsync(itemNecessario.IdItemEstoque);
            if (itemEstoque == null)
            {
                throw new KeyNotFoundException($"Item de estoque com id {request.IdOrdemServico} não encontrado");
            }
            
            itensNecessarios.Add(new ItemEstoqueOrdemServico.ItemNecessario(ordemServico.Id,
                itemEstoque.Codigo, itemEstoque.Nome, itemEstoque.UnidadeMedida, itemNecessario.Quantidade));
        }
        
        ordemServico.AdicionarItemServico(servico.Nome, request.ValorCobrado, itensNecessarios);
        await ordemServicoRepository.UpdateAsync(ordemServico);

        return new AdicionarItemOrdemServicoCommandResponse
        {
            Id = ordemServico.Id,
            Status = ordemServico.Status,
            ValorTotal = ordemServico.ValorTotal,
            RecebidaEm = ordemServico.RecebidaEm,
            Cliente = ordemServico.Cliente,
            Veiculo = ordemServico.Veiculo,
            Itens = ordemServico.ItensOrdemServico.ToList()
        };
    }
}