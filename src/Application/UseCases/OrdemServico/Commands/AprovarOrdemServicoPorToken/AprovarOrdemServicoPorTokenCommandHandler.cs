using Application.Abstractions.Gateways;
using Application.UseCases.OrdemServico.Commands.AprovarOrdemServico;
using MediatR;

namespace Application.UseCases.OrdemServico.Commands.AprovarOrdemServicoPorToken;

public class AprovarOrdemServicoPorTokenCommandHandler(
    IOrdemServicoGateway ordemServicoGateway,
    IMediator mediator
) : IRequestHandler<AprovarOrdemServicoPorTokenCommand, AprovarOrdemServicoCommandResponse>
{
    public async Task<AprovarOrdemServicoCommandResponse> Handle(
        AprovarOrdemServicoPorTokenCommand request,
        CancellationToken cancellationToken)
    {
        var ordemServico = await OrdemServicoPorTokenOwnership.ResolveAsync(
            ordemServicoGateway,
            request.TokenAprovacao,
            request.DocumentoCliente);

        return await mediator.Send(
            new AprovarOrdemServicoCommand { IdOrdemServico = ordemServico.Id },
            cancellationToken);
    }
}
