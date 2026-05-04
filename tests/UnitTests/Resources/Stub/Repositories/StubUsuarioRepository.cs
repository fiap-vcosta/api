using Application;
using Application.Repositories;
using Domain;
using Domain.Admin;

namespace UnitTests.Resources.Stub.Repositories;

public class StubUsuarioRepository(Usuario user) : IUsuarioRepository
{
    public Task<Usuario?> GetByIdAsync(int id) => Task.FromResult(user.Id == id ? user : null);
    public Task<IEnumerable<Usuario>> GetAllAsync() => Task.FromResult<IEnumerable<Usuario>>(new[] { user });
    public Task<Usuario?> GetByLoginAndPasswordAsync(string login, string password)
        => Task.FromResult(login == user.Login && password == user.Password ? user : null);
}
