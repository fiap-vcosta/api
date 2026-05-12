using Domain.OrdemServico.Entities;
using Domain.OrdemServico.ValueObjects;

namespace Application.Core.OrdemServico.Commands.AdicionarItemOrdemServico;

public class AdicionarItemOrdemServicoCommandResponse
{
    public int Id { get; init; }
    public StatusOrdemServico Status { get; init; }
    public decimal ValorTotal { get; init; }
    public DateTime RecebidaEm { get; init; }
    public required ClienteOrdemServico Cliente { get; init; }
    public required VeiculoOrdemServico Veiculo { get; init; }
    public required List<Servico> Itens { get; init; }
}