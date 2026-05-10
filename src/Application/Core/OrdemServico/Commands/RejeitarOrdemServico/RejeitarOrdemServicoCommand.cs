using MediatR;

namespace Application.Core.OrdemServico.Commands.RejeitarOrdemServico;

public class RejeitarOrdemServicoCommand : IRequest<RejeitarOrdemServicoCommandResponse>
{
    public int IdOrdemServico { get; init; }
}