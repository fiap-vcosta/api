namespace Domain.Entities;

public class Servico
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public decimal PrecoPadrao { get; set; }
    public bool Ativo { get; set; }
}
