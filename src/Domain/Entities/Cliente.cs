namespace Domain.Entities;

public enum TipoDocumento
{
    Cpf,
    Cnpj
}

public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TipoDocumento TipoDocumento { get; set; }
    public string Documento { get; set; } = string.Empty;
}
