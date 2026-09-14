using Application.Abstractions.Gateways;
using Application.Abstractions.Services;
using Application.UseCases.OrdemServico;
using Domain.Exceptions;
using MediatR;

namespace Application.UseCases.OrdemServico.Commands.ConfirmarPagamentoOrdemServico;

public class ConfirmarPagamentoOrdemServicoCommandHandler(
    IOrdemServicoGateway ordemServicoGateway,
    IOsMetrics osMetrics
) : IRequestHandler<ConfirmarPagamentoOrdemServicoCommand, OrdemServicoResponse>
{
    public async Task<OrdemServicoResponse> Handle(ConfirmarPagamentoOrdemServicoCommand request, CancellationToken cancellationToken)
    {
        var ordemServico = await ordemServicoGateway.GetByIdAsync(request.IdOrdemServico);
        if (ordemServico == null)
        {
            throw new DomainNotFoundException($"Ordem de Serviço com id {request.IdOrdemServico} não encontrada");
        }

        var statusAnterior = ordemServico.Status;
        ordemServico.ConfirmarPagamento();
        await ordemServicoGateway.UpdateAsync(ordemServico);
        OrdemServicoStatusMetrics.EmitIfChanged(osMetrics, ordemServico, statusAnterior);

        return OrdemServicoResponse.From(ordemServico);
    }
}
