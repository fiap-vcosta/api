namespace Domain.Entities;

public class Veiculo
{
    public int Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public int DonoId { get; set; }
    public Cliente Dono { get; set; } = null!;
    public string Modelo { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
}
