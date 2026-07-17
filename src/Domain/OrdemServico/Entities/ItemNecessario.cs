using Domain.Exceptions;
using Domain.OrdemServico.ValueObjects;

namespace Domain.OrdemServico.Entities;

public enum StatusItemEstoque
{
    EstoqueNaoChecado,
    EstoqueEmFalta,
    EstoqueDisponivel,
    EstoqueTravado,
    Utilizado
}

public class ItemNecessario
{
    public int Id {  get; private set; }
    public int IdOrdemServico { get; private set; }
    public int IdItemOrdemServico { get; private set; }
    public StatusItemEstoque Status { get; private set; }
    public decimal Quantidade { get; init; }
    public required ItemEstoqueOrdemServico ItemEstoque { get; init; }
    
    public record CriarItemNecessarioParams(int idOrdemServico, decimal quantidade, ItemEstoqueOrdemServico itemEstoque);
    public static ItemNecessario Criar(CriarItemNecessarioParams @params) => new()
    {
        IdOrdemServico = @params.idOrdemServico,
        Status = StatusItemEstoque.EstoqueNaoChecado,
        Quantidade = @params.quantidade,
        ItemEstoque = @params.itemEstoque
    };

    public void ChecarEstoque(decimal quantidadeDisponivel)
    {
        if (Status is not (StatusItemEstoque.EstoqueNaoChecado or StatusItemEstoque.EstoqueEmFalta or StatusItemEstoque.EstoqueDisponivel))
        {
            throw new BusinessRuleException($"Item de estoque {Id} com status {Status} não pode ser checado no estoque.");
        }

        if (Quantidade > quantidadeDisponivel)
        {
            Status = StatusItemEstoque.EstoqueEmFalta;
            return;
        }

        Status = StatusItemEstoque.EstoqueDisponivel;
    }

    public void TravarEstoque()
    {
        if (Status is not (StatusItemEstoque.EstoqueNaoChecado or StatusItemEstoque.EstoqueEmFalta or StatusItemEstoque.EstoqueDisponivel))
        {
            throw new BusinessRuleException($"Item de estoque {Id} com status {Status} não pode ter estoque travado.");
        }

        Status = StatusItemEstoque.EstoqueTravado;
    }

    public void ConfirmarUtilizacao()
    {
        if (Status is not StatusItemEstoque.EstoqueTravado)
        {
            throw new BusinessRuleException($"Item de estoque {Id} com status {Status} não pode ser utilizado.");
        }

        Status = StatusItemEstoque.Utilizado;
    }
}