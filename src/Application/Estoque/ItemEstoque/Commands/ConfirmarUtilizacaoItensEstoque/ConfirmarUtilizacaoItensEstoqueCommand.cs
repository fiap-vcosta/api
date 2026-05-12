using MediatR;

namespace Application.Estoque.ItemEstoque.Commands.ConfirmarUtilizacaoItensEstoque;

public class ConfirmarUtilizacaoItensEstoqueCommand : IRequest<Unit>
{
    public int IdOrdemServico { get; init; }
}