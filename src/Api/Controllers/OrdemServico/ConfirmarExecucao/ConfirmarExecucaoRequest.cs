using Domain.OrdemServico.ValueObjects;

namespace Api.Controllers.OrdemServico.ConfirmarExecucao;

public class ConfirmarExecucaoRequest
{
    public List<ServicoExecutado> ServicosExecutados { get; init; } = [];
}