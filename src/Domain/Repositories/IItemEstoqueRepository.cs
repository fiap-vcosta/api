using Domain.Entities;

namespace Domain.Repositories;

public interface IItemEstoqueRepository
{
    Task<IEnumerable<ItemEstoque>> GetAllAsync();
    Task<ItemEstoque?> GetByIdAsync(int id);
    Task<ItemEstoque?> GetByCodigoAsync(string codigo);
    Task CreateAsync(ItemEstoque itemEstoque);
    Task UpdateAsync(ItemEstoque itemEstoque);
    Task DeleteAsync(int id);
}
