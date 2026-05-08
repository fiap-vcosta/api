using Domain.Estoque.Entities;

namespace Domain.Administrativo.Entities;

public class ServicoAggregateRoot
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public decimal PrecoPadrao { get; set; }
    public bool Ativo { get; set; }
    public ICollection<ItemEstoqueAggregateRoot> ItensNecessarios { get; init; } = new List<ItemEstoqueAggregateRoot>();
}
