using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByIdAsync(int id);
    Task<IEnumerable<Usuario>> GetAllAsync();
    Task<Usuario?> GetByLoginAsync(string login);
}
