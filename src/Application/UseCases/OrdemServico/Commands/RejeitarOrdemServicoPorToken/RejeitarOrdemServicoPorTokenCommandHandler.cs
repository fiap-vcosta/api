using Application.Abstractions.Gateways;
using Application.UseCases.OrdemServico.Commands.RejeitarOrdemServico;
using MediatR;

namespace Application.UseCases.OrdemServico.Commands.RejeitarOrdemServicoPorToken;

public class RejeitarOrdemServicoPorTokenCommandHandler(
    IOrdemServicoGateway ordemServicoGateway,
    IMediator mediator
) : IRequestHandler<RejeitarOrdemServicoPorTokenCommand, RejeitarOrdemServicoCommandResponse>
{
    public async Task<RejeitarOrdemServicoCommandResponse> Handle(
        RejeitarOrdemServicoPorTokenCommand request,
        CancellationToken cancellationToken)
    {
        var ordemServico = await OrdemServicoPorTokenOwnership.ResolveAsync(
            ordemServicoGateway,
            request.TokenAprovacao,
            request.DocumentoCliente);

        return await mediator.Send(
            new RejeitarOrdemServicoCommand { IdOrdemServico = ordemServico.Id },
            cancellationToken);
    }
}
