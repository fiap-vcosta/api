using MediatR;

namespace Application.Core.OrdemServico.Commands.CriarOrdemServico;

public class CriarOrdemServicoCommand : IRequest<CriarOrdemServicoCommandResponse>
{
    public int IdVeiculo { get; init; }
}