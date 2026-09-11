using Application.Abstractions.Gateways;
using Application.UseCases.OrdemServico.Commands.AprovarOrdemServico;
using Domain.Exceptions;
using MediatR;

namespace Application.UseCases.OrdemServico.Commands.AprovarOrdemServicoPorToken;

public class AprovarOrdemServicoPorTokenCommandHandler(
    IOrdemServicoGateway ordemServicoGateway,
    IClienteGateway clienteGateway,
    IMediator mediator
) : IRequestHandler<AprovarOrdemServicoPorTokenCommand, AprovarOrdemServicoCommandResponse>
{
    private const string TokenNaoEncontradoMessage = "Ordem de Serviço não encontrada para o token informado";

    public async Task<AprovarOrdemServicoCommandResponse> Handle(
        AprovarOrdemServicoPorTokenCommand request,
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

        return await mediator.Send(new AprovarOrdemServicoCommand { IdOrdemServico = ordemServico.Id }, cancellationToken);
    }
}
