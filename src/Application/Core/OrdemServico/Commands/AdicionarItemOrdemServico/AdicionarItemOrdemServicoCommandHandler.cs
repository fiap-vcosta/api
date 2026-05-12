using Domain.Administrativo.Repositories;
using Domain.Estoque.Repositories;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Repositories;
using Domain.OrdemServico.ValueObjects;
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

        var itensNecessarios = new List<ItemNecessario.CriarItemNecessarioParams>();
        foreach (var itemNecessario in request.ItensNecessarios)
        {
            var itemEstoque = await itemEstoqueRepository.GetByIdAsync(itemNecessario.IdItemEstoque);
            if (itemEstoque == null)
            {
                throw new KeyNotFoundException($"Item de estoque com id {request.IdOrdemServico} não encontrado");
            }

            var itemEstoqueOrdemServico = new ItemEstoqueOrdemServico
            {
                Id = itemEstoque.Id,
                Codigo = itemEstoque.Codigo,
                Nome = itemEstoque.Nome,
                UnidadeMedida = itemEstoque.UnidadeMedida.ToString()
            };

            itensNecessarios.Add(new ItemNecessario.CriarItemNecessarioParams(ordemServico.Id, itemNecessario.Quantidade, itemEstoqueOrdemServico));
        }

        var servicoCatalogo = new ServicoCatalogo
        {
            Id = servico.Id,
            Nome = servico.Nome,
            Codigo = servico.Codigo
        };

        ordemServico.AdicionarItemServico(servico.Nome, request.ValorCobrado, servicoCatalogo, itensNecessarios);
        await ordemServicoRepository.UpdateAsync(ordemServico);

        return new AdicionarItemOrdemServicoCommandResponse
        {
            Id = ordemServico.Id,
            Status = ordemServico.Status,
            ValorTotal = ordemServico.ValorTotal,
            RecebidaEm = ordemServico.RecebidaEm,
            Cliente = ordemServico.Cliente,
            Veiculo = ordemServico.Veiculo,
            Itens = ordemServico.Servicos.ToList()
        };
    }
}