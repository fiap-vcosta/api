using Application.UseCases.OrdemServico.Commands.AprovarOrdemServico;
using Application.UseCases.OrdemServico.Queries.GetOrdemServicoByTokenEDocumento;
using MediatR;

namespace Application.UseCases.OrdemServico.Commands.AprovarOrdemServicoPorToken;

public class AprovarOrdemServicoPorTokenCommandHandler(IMediator mediator)
    : IRequestHandler<AprovarOrdemServicoPorTokenCommand, AprovarOrdemServicoCommandResponse>
{
    public async Task<AprovarOrdemServicoCommandResponse> Handle(
        AprovarOrdemServicoPorTokenCommand request,
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
            new AprovarOrdemServicoCommand { IdOrdemServico = idOrdemServico },
            cancellationToken);
    }
}
