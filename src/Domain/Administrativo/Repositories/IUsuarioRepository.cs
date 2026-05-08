using Domain.Administrativo.Entities;

namespace Domain.Administrativo.Repositories;

public interface IUsuarioRepository
{
    Task<UsuarioAggregateRoot?> GetByIdAsync(int id);
    Task<IEnumerable<UsuarioAggregateRoot>> GetAllAsync();
    Task<UsuarioAggregateRoot?> GetByLoginAndPasswordAsync(string login, string password);
}
