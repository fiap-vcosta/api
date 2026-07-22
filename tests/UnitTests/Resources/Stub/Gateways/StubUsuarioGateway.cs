using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;

namespace UnitTests.Resources.Stub.Gateways;

public class StubUsuarioGateway(UsuarioAggregateRoot user) : IUsuarioGateway
{
    public Task<UsuarioAggregateRoot?> GetByIdAsync(int id) => Task.FromResult(user.Id == id ? user : null);
    public Task<IEnumerable<UsuarioAggregateRoot>> GetAllAsync() => Task.FromResult<IEnumerable<UsuarioAggregateRoot>>([user]);
    public Task<UsuarioAggregateRoot?> GetByLoginAndPasswordAsync(string login, string password)
        => Task.FromResult(login == user.Login && password == user.Password ? user : null);
}
