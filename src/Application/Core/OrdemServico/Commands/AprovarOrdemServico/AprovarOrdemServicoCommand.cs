using MediatR;

namespace Application.Core.OrdemServico.Commands.AprovarOrdemServico;

public class AprovarOrdemServicoCommand : IRequest<AprovarOrdemServicoCommandResponse>
{
    public int IdOrdemServico { get; init; }
}