using Domain.OrdemServico.Entities;

namespace Application.Abstractions.Gateways;

public interface IOrdemServicoGateway
{
    Task<OrdemServicoAggregateRoot?> GetByIdAsync(int IdOrdemServico);
    Task<IEnumerable<OrdemServicoAggregateRoot>> GetAguardandoPecaPorItemEstoqueAsync(int idItemEstoque);
    Task CriarAsync(OrdemServicoAggregateRoot ordemServico);
    Task UpdateAsync(OrdemServicoAggregateRoot ordemServico);
}
