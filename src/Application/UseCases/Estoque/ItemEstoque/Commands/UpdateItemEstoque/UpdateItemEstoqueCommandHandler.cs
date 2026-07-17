using Application.UseCases.Estoque.ItemEstoque.Responses;
using Domain.Exceptions;

using Application.Abstractions.Gateways;
using MediatR;

namespace Application.UseCases.Estoque.ItemEstoque.Commands.UpdateItemEstoque;

public class UpdateItemEstoqueCommandHandler(IItemEstoqueGateway itemEstoqueGateway)
    : IRequestHandler<UpdateItemEstoqueCommand, ItemEstoqueResponse>
{
    public async Task<ItemEstoqueResponse> Handle(UpdateItemEstoqueCommand request, CancellationToken cancellationToken)
    {
        var item = await itemEstoqueGateway.GetByIdAsync(request.Id);
        if (item == null)
        {
            throw new DomainNotFoundException($"Item de estoque com id {request.Id} não encontrado");
        }

        var existing = await itemEstoqueGateway.GetByCodigoAsync(request.Codigo);
        if (existing != null && existing.Id != item.Id)
        {
            throw new BusinessRuleException("Já existe um item de estoque com este código.");
        }

        item.Codigo = request.Codigo;
        item.Tipo = request.Tipo;
        item.Nome = request.Nome;
        item.UnidadeMedida = request.UnidadeMedida;
        item.PrecoVenda = request.PrecoVenda;

        await itemEstoqueGateway.UpdateAsync(item);

        return new ItemEstoqueResponse
        {
            Id = item.Id,
            Codigo = item.Codigo,
            Tipo = item.Tipo,
            Nome = item.Nome,
            UnidadeMedida = item.UnidadeMedida,
            PrecoVenda = item.PrecoVenda,
            Saldo = item.Saldo,
            SaldoReservado = item.SaldoReservado
        };
    }
}
