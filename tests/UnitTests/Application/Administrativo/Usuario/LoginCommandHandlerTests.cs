using Application.Administrativo.Usuario.Commands.Login;
using Domain.Administrativo.Entities;
using UnitTests.Resources.Stub.Repositories;
using UnitTests.Resources.Stub.Services;

namespace UnitTests.Application.Administrativo.Usuario;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsToken_WhenCredentialsAreValid()
    {
        // Arrange
        var repository = new StubUsuarioRepository(new UsuarioAggregateRoot
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
        var token = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("admin-Admin-1", token);
    }

    [Fact]
    public async Task Handle_ThrowsUnauthorizedAccessException_WhenCredentialsAreInvalid()
    {
        // Arrange
        var repository = new StubUsuarioRepository(new UsuarioAggregateRoot
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
            await handler.Handle(command, CancellationToken.None));
    }
}
