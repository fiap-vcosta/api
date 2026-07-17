using MediatR;

namespace Application.UseCases.OrdemServico.Commands.CriarOrdemServico;

public class CriarOrdemServicoCommand : IRequest<CriarOrdemServicoCommandResponse>
{
    public int IdVeiculo { get; init; }
}