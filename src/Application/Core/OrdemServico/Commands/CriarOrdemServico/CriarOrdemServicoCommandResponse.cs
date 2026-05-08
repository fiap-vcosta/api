using Domain.OrdemServico.Entities;
using Domain.OrdemServico.ValueObjects;

namespace Application.Core.OrdemServico.Commands.CriarOrdemServico;

public class CriarOrdemServicoCommandResponse
{
    public int Id { get; init; }
    public StatusOrdemServico StatusOrdemServico { get; init; }
    public DateTime RecebidaEm { get; init; }
    public required ClienteOrdemServico Cliente { get; init; }
    public required VeiculoOrdemServico Veiculo { get; init; }
}