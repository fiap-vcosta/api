using MediatR;

namespace Application.Core.OrdemServico.Commands.AprovarServicosParcialmente;

public class AprovarServicosParcialmenteCommand : IRequest<AprovarServicosParcialmenteCommandResponse>
{
    public int IdOrdemServico { get; init; }
    public List<int> IdServicosAprovados { get; init; } = [];
}