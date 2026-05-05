using Domain.Entities;

namespace Domain.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByIdAsync(int id);
    Task<IEnumerable<Usuario>> GetAllAsync();
    Task<Usuario?> GetByLoginAndPasswordAsync(string login, string password);
}
