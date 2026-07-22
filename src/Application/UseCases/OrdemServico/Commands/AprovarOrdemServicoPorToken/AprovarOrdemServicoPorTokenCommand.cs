using Application.UseCases.OrdemServico.Commands.AprovarOrdemServico;
using MediatR;

namespace Application.UseCases.OrdemServico.Commands.AprovarOrdemServicoPorToken;

public class AprovarOrdemServicoPorTokenCommand : IRequest<AprovarOrdemServicoCommandResponse>
{
    public required string TokenAprovacao { get; init; }
}
