using Domain.Estoque.Entities;

namespace Application.Abstractions.Gateways;

public interface IItemEstoqueGateway
{
    Task<IEnumerable<ItemEstoqueAggregateRoot>> GetAllAsync();
    Task<ItemEstoqueAggregateRoot?> GetByIdAsync(int id);
    Task<ItemEstoqueAggregateRoot?> GetByCodigoAsync(string codigo);
    Task<IEnumerable<ItemEstoqueAggregateRoot>> GetEBloquearItensAsync(List<int> ids);
    Task<IEnumerable<ItemEstoqueAggregateRoot>> GetUtilizadosByOrdemServico(int idOrdemServico);
    Task CreateAsync(ItemEstoqueAggregateRoot itemEstoqueAggregateRoot);
    Task UpdateAsync(ItemEstoqueAggregateRoot itemEstoqueAggregateRoot);
    Task DeleteAsync(int id);
}
