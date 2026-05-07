using Domain.Estoque.Entities;

namespace Domain.Administrativo.Entities;

public class Servico
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public decimal PrecoPadrao { get; set; }
    public bool Ativo { get; set; }
    public ICollection<ItemEstoque> ItensNecessarios { get; init; } = new List<ItemEstoque>();
}
