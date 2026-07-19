using Application.UseCases.OrdemServico.Commands.RejeitarOrdemServico;
using MediatR;

namespace Application.UseCases.OrdemServico.Commands.RejeitarOrdemServicoPorToken;

public class RejeitarOrdemServicoPorTokenCommand : IRequest<RejeitarOrdemServicoCommandResponse>
{
    public required string TokenAprovacao { get; init; }
}
