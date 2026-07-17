using Domain.Administrativo.Entities;

namespace Application.Abstractions.Gateways;

public interface IVeiculoGateway
{
    Task<IEnumerable<VeiculoAggregateRoot>> GetAllAsync();
    Task<VeiculoAggregateRoot?> GetByIdAsync(int id);
    Task<IEnumerable<VeiculoAggregateRoot>> GetByDonoIdAsync(int donoId);
    Task<VeiculoAggregateRoot?> GetByPlacaAsync(string placa);
    Task CreateAsync(VeiculoAggregateRoot veiculoAggregateRoot);
    Task UpdateAsync(VeiculoAggregateRoot veiculoAggregateRoot);
    Task DeleteAsync(int id);
}
