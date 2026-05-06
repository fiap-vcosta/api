using Domain.Entities;

namespace Domain.Repositories;

public interface IServicoRepository
{
    Task<IEnumerable<Servico>> GetAllAsync();
    Task<Servico?> GetByIdAsync(int id);
    Task<Servico?> GetByCodigoAsync(string codigo);
    Task CreateAsync(Servico servico);
    Task UpdateAsync(Servico servico);
    Task DeleteAsync(int id);
}
