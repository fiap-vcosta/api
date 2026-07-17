using MediatR;

namespace Application.UseCases.OrdemServico.Commands.RejeitarOrdemServico;

public class RejeitarOrdemServicoCommand : IRequest<RejeitarOrdemServicoCommandResponse>
{
    public int IdOrdemServico { get; init; }
}