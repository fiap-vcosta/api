namespace Domain;

public enum TipoUsuario
{
    Admin,
    Atendente,
    Mecanico,
    Cliente
}

public class Usuario
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public TipoUsuario TipoUsuario { get; set; }
}
