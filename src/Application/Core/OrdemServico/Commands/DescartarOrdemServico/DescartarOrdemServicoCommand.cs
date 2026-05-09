using MediatR;

namespace Application.Core.OrdemServico.Commands.DescartarOrdemServico;

public class DescartarOrdemServicoCommand : IRequest<DescartarOrdemServicoResponse>
{
    public int IdOrdemServico { get; init; }
}