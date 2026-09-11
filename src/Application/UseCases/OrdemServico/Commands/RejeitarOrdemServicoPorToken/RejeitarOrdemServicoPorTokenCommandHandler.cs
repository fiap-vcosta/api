using Application.Abstractions.Gateways;
using Application.UseCases.OrdemServico.Commands.RejeitarOrdemServico;
using Domain.Exceptions;
using MediatR;

namespace Application.UseCases.OrdemServico.Commands.RejeitarOrdemServicoPorToken;

public class RejeitarOrdemServicoPorTokenCommandHandler(
    IOrdemServicoGateway ordemServicoGateway,
    IClienteGateway clienteGateway,
    IMediator mediator
) : IRequestHandler<RejeitarOrdemServicoPorTokenCommand, RejeitarOrdemServicoCommandResponse>
{
    private const string TokenNaoEncontradoMessage = "Ordem de Serviço não encontrada para o token informado";

    public async Task<RejeitarOrdemServicoCommandResponse> Handle(
        RejeitarOrdemServicoPorTokenCommand request,
        CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoGateway.GetByTokenAsync(request.TokenAprovacao);
        if (ordemServico is null)
        {
            throw new DomainNotFoundException(TokenNaoEncontradoMessage);
        }

        var cliente = await clienteGateway.GetByIdAsync(ordemServico.Cliente.Id);
        if (cliente is null || !OwnershipDocumento.Matches(request.DocumentoCliente, cliente.Documento))
        {
            throw new DomainNotFoundException(TokenNaoEncontradoMessage);
        }

        return await mediator.Send(new RejeitarOrdemServicoCommand { IdOrdemServico = ordemServico.Id }, cancellationToken);
    }
}
