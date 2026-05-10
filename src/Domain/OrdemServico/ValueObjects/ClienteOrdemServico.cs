namespace Domain.OrdemServico.ValueObjects;

public class ClienteOrdemServico
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}