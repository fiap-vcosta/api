using Domain.Administrativo.Entities;

namespace Domain.Administrativo.Repositories;

public interface IClienteRepository
{
    Task<IEnumerable<ClienteAggregateRoot>> GetAllAsync();
    Task<ClienteAggregateRoot?> GetByIdAsync(int id);
    Task<ClienteAggregateRoot?> GetByDocumentoAsync(string documento);
    Task CreateAsync(ClienteAggregateRoot clienteAggregateRoot);
    Task UpdateAsync(ClienteAggregateRoot clienteAggregateRoot);
    Task DeleteAsync(int id);
}
