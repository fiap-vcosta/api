namespace Domain.Administrativo.Entities;

public enum TipoUsuario
{
    Admin,
    Atendente,
    Mecanico,
    Cliente
}

public class Usuario
{
    public int Id { get; init; }
    public string Login { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public TipoUsuario TipoUsuario { get; init; }
}
