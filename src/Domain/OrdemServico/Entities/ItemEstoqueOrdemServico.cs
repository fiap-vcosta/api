using Domain.Estoque.Entities;

namespace Domain.OrdemServico.Entities;

public class ItemEstoqueOrdemServico
{
    public int Id {  get; private set; }
    public int IdOrdemServico { get; private set; }
    public int IdItemOrdemServico { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public string Nome { get; private set; } = string.Empty;
    public UnidadeMedida UnidadeMedida { get; private set; }
    public decimal Quantidade { get; private set; }

    public record ItemNecessario(int idOrdemServico, string codigo, string nome, UnidadeMedida unidadeMedida, decimal quantidade);
    public static ItemEstoqueOrdemServico Criar(ItemNecessario itemNecessario) => new()
    {
        IdOrdemServico = itemNecessario.idOrdemServico,
        Codigo = itemNecessario.codigo,
        Nome = itemNecessario.nome,
        UnidadeMedida = itemNecessario.unidadeMedida,
        Quantidade = itemNecessario.quantidade
    };
}