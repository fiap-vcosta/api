namespace Api.ViewModels.Servico;

public record ServicoViewModel
{
    public required int Id { get; init; }
    public required string Codigo { get; init; }
    public required string Nome { get; init; }
    public required decimal PrecoPadrao { get; init; }
    public required bool Ativo { get; init; }
}
