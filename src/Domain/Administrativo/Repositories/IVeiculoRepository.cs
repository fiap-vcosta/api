using Domain.Administrativo.Entities;

namespace Domain.Administrativo.Repositories;

public interface IVeiculoRepository
{
    Task<IEnumerable<Veiculo>> GetAllAsync();
    Task<Veiculo?> GetByIdAsync(int id);
    Task<IEnumerable<Veiculo>> GetByDonoIdAsync(int donoId);
    Task<Veiculo?> GetByPlacaAsync(string placa);
    Task CreateAsync(Veiculo veiculo);
    Task UpdateAsync(Veiculo veiculo);
    Task DeleteAsync(int id);
}
