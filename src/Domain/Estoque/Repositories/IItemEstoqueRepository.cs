using Domain.Estoque.Entities;

namespace Domain.Estoque.Repositories;

public interface IItemEstoqueRepository
{
    Task<IEnumerable<ItemEstoqueAggregateRoot>> GetAllAsync();
    Task<ItemEstoqueAggregateRoot?> GetByIdAsync(int id);
    Task<ItemEstoqueAggregateRoot?> GetByCodigoAsync(string codigo);
    Task CreateAsync(ItemEstoqueAggregateRoot itemEstoqueAggregateRoot);
    Task UpdateAsync(ItemEstoqueAggregateRoot itemEstoqueAggregateRoot);
    Task DeleteAsync(int id);
}
