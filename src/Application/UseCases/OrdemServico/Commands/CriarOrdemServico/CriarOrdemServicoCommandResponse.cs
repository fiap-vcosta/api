using Application.UseCases.OrdemServico.Responses;
using Domain.OrdemServico.Entities;

namespace Application.UseCases.OrdemServico.Commands.CriarOrdemServico;

public class CriarOrdemServicoCommandResponse
{
    public int Id { get; init; }
    public StatusOrdemServico Status { get; init; }
    public DateTime RecebidaEm { get; init; }
    public required ClienteOrdemServicoResponse Cliente { get; init; }
    public required VeiculoOrdemServicoResponse Veiculo { get; init; }
}
