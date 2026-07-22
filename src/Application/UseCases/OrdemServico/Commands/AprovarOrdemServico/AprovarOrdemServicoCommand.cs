using MediatR;

namespace Application.UseCases.OrdemServico.Commands.AprovarOrdemServico;

public class AprovarOrdemServicoCommand : IRequest<AprovarOrdemServicoCommandResponse>
{
    public int IdOrdemServico { get; init; }
}