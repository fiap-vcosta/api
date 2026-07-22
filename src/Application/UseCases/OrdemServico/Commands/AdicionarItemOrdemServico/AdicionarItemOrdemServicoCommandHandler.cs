using Application.Abstractions.Gateways;
using Application.UseCases.OrdemServico.Responses;
using Domain.Exceptions;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.ValueObjects;
using MediatR;

namespace Application.UseCases.OrdemServico.Commands.AdicionarItemOrdemServico;

public class AdicionarItemOrdemServicoCommandHandler(
    IOrdemServicoGateway ordemServicoGateway,
    IServicoGateway servicoGateway,
    IItemEstoqueGateway itemEstoqueGateway
) : IRequestHandler<AdicionarItemOrdemServicoCommand, AdicionarItemOrdemServicoCommandResponse>
{
    public async Task<AdicionarItemOrdemServicoCommandResponse> Handle(AdicionarItemOrdemServicoCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoGateway.GetByIdAsync(request.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new DomainNotFoundException($"Ordem de Serviço com id {request.IdOrdemServico} não encontrada");
        }
        
        var servico = await servicoGateway.GetByIdAsync(request.IdServico);
        if (servico == null)
        {
            throw new DomainNotFoundException($"Serviço com id {request.IdOrdemServico} não encontrado");
        }

        var itensNecessarios = new List<ItemNecessario.CriarItemNecessarioParams>();
        foreach (var itemNecessario in request.ItensNecessarios)
        {
            var itemEstoque = await itemEstoqueGateway.GetByIdAsync(itemNecessario.IdItemEstoque);
            if (itemEstoque == null)
            {
                throw new DomainNotFoundException($"Item de estoque com id {request.IdOrdemServico} não encontrado");
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
        await ordemServicoGateway.UpdateAsync(ordemServico);

        return new AdicionarItemOrdemServicoCommandResponse
        {
            Id = ordemServico.Id,
            Status = ordemServico.Status,
            ValorTotal = ordemServico.ValorTotal,
            RecebidaEm = ordemServico.RecebidaEm,
            Cliente = ClienteOrdemServicoResponse.From(ordemServico.Cliente),
            Veiculo = VeiculoOrdemServicoResponse.From(ordemServico.Veiculo),
            Itens = ServicoOrdemServicoResponse.FromMany(ordemServico.Servicos)
        };
    }
}