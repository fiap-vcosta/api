using Domain.Administrativo.Entities;

namespace Domain.Administrativo.Repositories;

public interface IServicoRepository
{
    Task<IEnumerable<ServicoAggregateRoot>> GetAllAsync();
    Task<ServicoAggregateRoot?> GetByIdAsync(int id);
    Task<ServicoAggregateRoot?> GetByCodigoAsync(string codigo);
    Task CreateAsync(ServicoAggregateRoot servicoAggregateRoot);
    Task UpdateAsync(ServicoAggregateRoot servicoAggregateRoot);
    Task DeleteAsync(int id);
}
