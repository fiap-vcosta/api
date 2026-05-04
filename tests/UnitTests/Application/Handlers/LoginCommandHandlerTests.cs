using Application;
using Application.Commands;
using Application.Handlers;
using Application.Services;
using Domain;
using Domain.Admin;
using UnitTests.Resources.Stub.Services;
using UnitTests.Resources.Stub.Repositories;
using Xunit;

namespace UnitTests.Application.Handlers;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsToken_WhenCredentialsAreValid()
    {
        var repository = new StubUsuarioRepository(new Usuario
        {
            Id = 1,
            Login = "admin",
            Password = "secret",
            TipoUsuario = TipoUsuario.Admin
        });
        var jwtService = new StubJwtService();
        var handler = new LoginCommandHandler(repository, jwtService);

        var token = await handler.Handle(new LoginCommand { Login = "admin", Password = "secret" }, default);

        Assert.Equal("admin-Admin-1", token);
    }

    [Fact]
    public async Task Handle_ThrowsUnauthorizedAccessException_WhenCredentialsAreInvalid()
    {
        var repository = new StubUsuarioRepository(new Usuario
        {
            Id = 1,
            Login = "admin",
            Password = "secret",
            TipoUsuario = TipoUsuario.Admin
        });
        var jwtService = new StubJwtService();
        var handler = new LoginCommandHandler(repository, jwtService);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await handler.Handle(new LoginCommand { Login = "admin", Password = "wrong" }, default));
    }
}
