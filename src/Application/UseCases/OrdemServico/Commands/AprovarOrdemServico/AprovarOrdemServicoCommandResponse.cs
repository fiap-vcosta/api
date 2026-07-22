using Application.UseCases.OrdemServico.Responses;
using Domain.OrdemServico.Entities;

namespace Application.UseCases.OrdemServico.Commands.AprovarOrdemServico;

public class AprovarOrdemServicoCommandResponse
{
    public int Id { get; init; }
    public StatusOrdemServico Status { get; init; }
    public decimal ValorTotal { get; init; }
    public DateTime RecebidaEm { get; init; }
    public DateTime AprovadaEm { get; init; }
    public required ClienteOrdemServicoResponse Cliente { get; init; }
    public required VeiculoOrdemServicoResponse Veiculo { get; init; }
    public required List<ServicoOrdemServicoResponse> Servicos { get; init; }
}
