namespace Domain.Administrativo.Entities;

public enum TipoUsuario
{
    Admin,
    Atendente,
    Mecanico,
    Cliente
}

public class UsuarioAggregateRoot
{
    public int Id { get; init; }
    public string Login { get; init; } = string.Empty;
    public string Senha { get; init; } = string.Empty;
    public TipoUsuario TipoUsuario { get; init; }
}
