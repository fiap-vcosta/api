using Domain.OrdemServico.Entities;

namespace Application.Abstractions.Gateways;

public interface IOrdemServicoGateway
{
    Task<OrdemServicoAggregateRoot?> GetByIdAsync(int idOrdemServico);
    Task<IEnumerable<OrdemServicoAggregateRoot>> GetAguardandoPecaPorItemEstoqueAsync(int idItemEstoque);
    Task CriarAsync(OrdemServicoAggregateRoot ordemServico);
    Task UpdateAsync(OrdemServicoAggregateRoot ordemServico);
}
