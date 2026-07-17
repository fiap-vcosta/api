namespace Api.ViewModels.OrdemServico;

public record TempoMedioExecucaoViewModel
{
    public required int IdServico { get; init; }
    public required int TotalExecucoes { get; init; }
    public required TimeSpan ExecucaoMedia { get; init; }
}
