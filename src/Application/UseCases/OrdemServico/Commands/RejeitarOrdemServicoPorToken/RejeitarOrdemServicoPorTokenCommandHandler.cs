using Application.Abstractions.Gateways;
using Application.UseCases.OrdemServico.Commands.RejeitarOrdemServico;
using Domain.Exceptions;
using MediatR;

namespace Application.UseCases.OrdemServico.Commands.RejeitarOrdemServicoPorToken;

public class RejeitarOrdemServicoPorTokenCommandHandler(
    IOrdemServicoGateway ordemServicoGateway,
    IMediator mediator
) : IRequestHandler<RejeitarOrdemServicoPorTokenCommand, RejeitarOrdemServicoCommandResponse>
{
    public async Task<RejeitarOrdemServicoCommandResponse> Handle(RejeitarOrdemServicoPorTokenCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoGateway.GetByTokenAsync(request.TokenAprovacao);
        if (ordemServico == null)
        {
            throw new DomainNotFoundException("Ordem de Serviço não encontrada para o token informado");
        }

        return await mediator.Send(new RejeitarOrdemServicoCommand { IdOrdemServico = ordemServico.Id }, cancellationToken);
    }
}
