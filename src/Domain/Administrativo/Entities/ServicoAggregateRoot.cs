namespace Domain.Administrativo.Entities;

public class ServicoAggregateRoot
{
    public record ItemNecessarioServico(int IdItemEstoque);
    
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public decimal PrecoPadrao { get; set; }
    public bool Ativo { get; set; }
    
    private readonly List<ItemNecessarioServico> _itensNecessarios = new();
    public IReadOnlyCollection<ItemNecessarioServico> ItensNecessarios => _itensNecessarios.AsReadOnly();
}
