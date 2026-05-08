namespace Domain.Administrativo.Entities;

public class VeiculoAggregateRoot
{
    public int Id { get; set; }
    public int IdDono { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
}
