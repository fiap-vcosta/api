using MediatR;

namespace Application.UseCases.OrdemServico.Commands.ConfirmarPagamentoOrdemServico;

public class ConfirmarPagamentoOrdemServicoCommand : IRequest<OrdemServicoResponse>
{
    public int IdOrdemServico { get; init; }
}
