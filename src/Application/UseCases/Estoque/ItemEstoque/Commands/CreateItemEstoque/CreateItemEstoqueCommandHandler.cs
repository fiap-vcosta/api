using Application.Abstractions.Gateways;
using Domain.Exceptions;
using Application.UseCases.Estoque.ItemEstoque.Responses;
using MediatR;

namespace Application.UseCases.Estoque.ItemEstoque.Commands.CreateItemEstoque;

public class CreateItemEstoqueCommandHandler(IItemEstoqueGateway itemEstoqueGateway)
    : IRequestHandler<CreateItemEstoqueCommand, ItemEstoqueResponse>
{
    public async Task<ItemEstoqueResponse> Handle(CreateItemEstoqueCommand request, CancellationToken cancellationToken)
    {
        var existing = await itemEstoqueGateway.GetByCodigoAsync(request.Codigo);
        if (existing != null)
        {
            throw new BusinessRuleException("Já existe um item de estoque com este código.");
        }

        var item = new Domain.Estoque.Entities.ItemEstoqueAggregateRoot
        {
            Codigo = request.Codigo,
            Tipo = request.Tipo,
            Nome = request.Nome,
            UnidadeMedida = request.UnidadeMedida,
            PrecoVenda = request.PrecoVenda,
            Saldo = request.Saldo,
            SaldoReservado = request.SaldoReservado
        };

        await itemEstoqueGateway.CreateAsync(item);

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
