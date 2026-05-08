namespace Domain.Administrativo.Entities;

public class VeiculoAggregateRoot
{
    public int Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public int DonoId { get; set; }
    public ClienteAggregateRoot Dono { get; init; } = null!;
    public string Modelo { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
}
