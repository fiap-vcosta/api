using Domain.Administrativo.Entities;

namespace Application.Abstractions.Gateways;

public interface IUsuarioGateway
{
    Task<UsuarioAggregateRoot?> GetByIdAsync(int id);
    Task<IEnumerable<UsuarioAggregateRoot>> GetAllAsync();
    Task<UsuarioAggregateRoot?> GetByLoginAndPasswordAsync(string login, string password);
}
