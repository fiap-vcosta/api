namespace Domain.Administrativo.Entities;

public enum TipoDocumento
{
    Cpf,
    Cnpj
}

public class ClienteAggregateRoot
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TipoDocumento TipoDocumento { get; set; }
    public string Documento { get; set; } = string.Empty;
}
