using Application.Abstractions.Gateways;
using Application.UseCases.OrdemServico.Responses;
using Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.OrdemServico.Commands.ConfirmarPagamentoOrdemServico;

public class ConfirmarPagamentoOrdemServicoCommandHandler(
    IOrdemServicoGateway ordemServicoGateway,
    ILogger<ConfirmarPagamentoOrdemServicoCommandHandler> logger
) : IRequestHandler<ConfirmarPagamentoOrdemServicoCommand, OrdemServicoResponse>
{
    public async Task<OrdemServicoResponse> Handle(ConfirmarPagamentoOrdemServicoCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoGateway.GetByIdAsync(request.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new DomainNotFoundException($"Ordem de Serviço com id {request.IdOrdemServico} não encontrada");
        }

        ordemServico.ConfirmarPagamento();
        await ordemServicoGateway.UpdateAsync(ordemServico);
        OrdemServicoStatusLog.Emit(logger, ordemServico.Id, ordemServico.Status);

        return OrdemServicoResponse.From(ordemServico);
    }
}
