using Application.Abstractions.Gateways;
using Domain.Exceptions;
using Domain.OrdemServico.Entities;

namespace Application.UseCases.OrdemServico;

internal static class OrdemServicoPorTokenOwnership
{
    private const string TokenNaoEncontradoMessage = "Ordem de Serviço não encontrada para o token informado";

    public static async Task<OrdemServicoAggregateRoot> ResolveAsync(
        IOrdemServicoGateway ordemServicoGateway,
        string tokenAprovacao,
        string documentoCliente)
    {
        var ordemServico = await ordemServicoGateway.GetByTokenEDocumentoAsync(tokenAprovacao, documentoCliente);
        if (ordemServico is null)
        {
            throw new DomainNotFoundException(TokenNaoEncontradoMessage);
        }

        return ordemServico;
    }
}
