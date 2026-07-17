using MediatR;

namespace Application.UseCases.OrdemServico.Commands.DescartarOrdemServico;

public class DescartarOrdemServicoCommand : IRequest<DescartarOrdemServicoResponse>
{
    public int IdOrdemServico { get; init; }
}