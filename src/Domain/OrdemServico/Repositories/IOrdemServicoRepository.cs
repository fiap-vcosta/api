using Domain.OrdemServico.Entities;

namespace Domain.OrdemServico.Repositories;

public interface IOrdemServicoRepository
{
    Task<OrdemServicoAggregateRoot?> GetByIdAsync(int IdOrdemServico);
    Task<IEnumerable<OrdemServicoAggregateRoot>> GetAguardandoPecaPorItemEstoqueAsync(int idItemEstoque);
    Task CriarAsync(OrdemServicoAggregateRoot ordemServico);
    Task UpdateAsync(OrdemServicoAggregateRoot ordemServico);
}