using Domain.Administrativo.Entities;

namespace Application.Abstractions.Gateways;

public interface IServicoGateway
{
    Task<IEnumerable<ServicoAggregateRoot>> GetAllAsync();
    Task<ServicoAggregateRoot?> GetByIdAsync(int id);
    Task<ServicoAggregateRoot?> GetByCodigoAsync(string codigo);
    Task CreateAsync(ServicoAggregateRoot servicoAggregateRoot);
    Task UpdateAsync(ServicoAggregateRoot servicoAggregateRoot);
    Task DeleteAsync(int id);
}
