using Application.UseCases.OrdemServico.Commands.RejeitarOrdemServico;
using Application.UseCases.OrdemServico.Queries.GetOrdemServicoByTokenEDocumento;
using MediatR;

namespace Application.UseCases.OrdemServico.Commands.RejeitarOrdemServicoPorToken;

public class RejeitarOrdemServicoPorTokenCommandHandler(IMediator mediator)
    : IRequestHandler<RejeitarOrdemServicoPorTokenCommand, RejeitarOrdemServicoCommandResponse>
{
    public async Task<RejeitarOrdemServicoCommandResponse> Handle(
        RejeitarOrdemServicoPorTokenCommand request,
        CancellationToken cancellationToken)
    {
        var idOrdemServico = await mediator.Send(
            new GetOrdemServicoByTokenEDocumentoQuery
            {
                TokenAprovacao = request.TokenAprovacao,
                DocumentoCliente = request.DocumentoCliente
            },
            cancellationToken);

        return await mediator.Send(
            new RejeitarOrdemServicoCommand { IdOrdemServico = idOrdemServico },
            cancellationToken);
    }
}
