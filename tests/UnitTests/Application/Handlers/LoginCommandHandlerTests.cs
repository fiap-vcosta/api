using Application.Commands;
using Application.Handlers;
using Domain.Admin;
using UnitTests.Resources.Stub.Services;
using UnitTests.Resources.Stub.Repositories;

namespace UnitTests.Application.Handlers;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsToken_WhenCredentialsAreValid()
    {
        // Arrange
        var repository = new StubUsuarioRepository(new Usuario
        {
            Id = 1,
            Login = "admin",
            Password = "secret",
            TipoUsuario = TipoUsuario.Admin
        });
        var jwtService = new StubJwtService();
        var handler = new LoginCommandHandler(repository, jwtService);
        var command = new LoginCommand { Login = "admin", Password = "secret" };

        // Act
        var token = await handler.Handle(command, default);

        // Assert
        Assert.Equal("admin-Admin-1", token);
    }

    [Fact]
    public async Task Handle_ThrowsUnauthorizedAccessException_WhenCredentialsAreInvalid()
    {
        // Arrange
        var repository = new StubUsuarioRepository(new Usuario
        {
            Id = 1,
            Login = "admin",
            Password = "secret",
            TipoUsuario = TipoUsuario.Admin
        });
        var jwtService = new StubJwtService();
        var handler = new LoginCommandHandler(repository, jwtService);
        var command = new LoginCommand { Login = "admin", Password = "wrong" };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await handler.Handle(command, default));
    }
}
