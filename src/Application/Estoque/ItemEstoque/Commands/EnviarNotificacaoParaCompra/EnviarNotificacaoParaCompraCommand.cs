using MediatR;

namespace Application.Estoque.ItemEstoque.Commands.EnviarNotificacaoParaCompra;

public class EnviarNotificacaoParaCompraCommand : IRequest<Unit>
{
    public int IdItemEstoque { get; init; }
    public int IdOrdemServico { get; init; }
    public string NomeItem { get; init; } = string.Empty;
    public decimal QuantidadeFaltando { get; init; }
}
