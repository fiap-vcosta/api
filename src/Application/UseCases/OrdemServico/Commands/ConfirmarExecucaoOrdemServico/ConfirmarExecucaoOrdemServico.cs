using Domain.OrdemServico.ValueObjects;
using MediatR;

namespace Application.UseCases.OrdemServico.Commands.ConfirmarExecucaoOrdemServico;

public class ConfirmarExecucaoOrdemServicoCommand : IRequest<OrdemServicoResponse>
{
    public int IdOrdemServico { get; init; }
    public List<ServicoExecutado> ServicosExecutados { get; init; } = [];
}
