namespace Api.Controllers.Servico.CreateServico;

public class CreateServicoRequest
{
    public string Codigo { get; init; } = string.Empty;
    public string Nome { get; init; } = string.Empty;
    public decimal PrecoPadrao { get; init; }
    public bool Ativo { get; init; }
}
