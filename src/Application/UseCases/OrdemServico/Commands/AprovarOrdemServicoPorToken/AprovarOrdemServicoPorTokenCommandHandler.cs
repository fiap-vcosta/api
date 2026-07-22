using Application.Abstractions.Gateways;
using Application.UseCases.OrdemServico.Commands.AprovarOrdemServico;
using Domain.Exceptions;
using MediatR;

namespace Application.UseCases.OrdemServico.Commands.AprovarOrdemServicoPorToken;

public class AprovarOrdemServicoPorTokenCommandHandler(
    IOrdemServicoGateway ordemServicoGateway,
    IMediator mediator
) : IRequestHandler<AprovarOrdemServicoPorTokenCommand, AprovarOrdemServicoCommandResponse>
{
    public async Task<AprovarOrdemServicoCommandResponse> Handle(AprovarOrdemServicoPorTokenCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoGateway.GetByTokenAsync(request.TokenAprovacao);
        if (ordemServico == null)
        {
            throw new DomainNotFoundException("Ordem de Serviço não encontrada para o token informado");
        }

        return await mediator.Send(new AprovarOrdemServicoCommand { IdOrdemServico = ordemServico.Id }, cancellationToken);
    }
}
