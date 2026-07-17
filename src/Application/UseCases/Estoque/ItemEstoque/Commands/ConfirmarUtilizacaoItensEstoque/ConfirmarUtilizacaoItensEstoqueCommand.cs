using MediatR;

namespace Application.UseCases.Estoque.ItemEstoque.Commands.ConfirmarUtilizacaoItensEstoque;

public class ConfirmarUtilizacaoItensEstoqueCommand : IRequest<Unit>
{
    public int IdOrdemServico { get; init; }
}