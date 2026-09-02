using Application.UseCases.Administrativo.Usuario.Commands.Login;
using Domain.Administrativo.Entities;
using UnitTests.Resources.Stub.Gateways;
using UnitTests.Resources.Stub.Services;

namespace UnitTests.Application.UseCases.Administrativo.Usuario.Commands.Login;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsToken_WhenCredentialsAreValid()
    {
        // Arrange
        var gateway = new StubUsuarioGateway(new UsuarioAggregateRoot
        {
            Id = 1,
            Login = "admin",
            Senha = "secret",
            TipoUsuario = TipoUsuario.Admin
        });
        var jwtService = new StubJwtService();
        var handler = new LoginCommandHandler(gateway, jwtService);
        var command = new LoginCommand { Login = "admin", Senha = "secret" };

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("admin-Admin-1", response.Token);
    }

    [Fact]
    public async Task Handle_ThrowsUnauthorizedAccessException_WhenCredentialsAreInvalid()
    {
        // Arrange
        var gateway = new StubUsuarioGateway(new UsuarioAggregateRoot
        {
            Id = 1,
            Login = "admin",
            Senha = "secret",
            TipoUsuario = TipoUsuario.Admin
        });
        var jwtService = new StubJwtService();
        var handler = new LoginCommandHandler(gateway, jwtService);
        var command = new LoginCommand { Login = "admin", Senha = "wrong" };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await handler.Handle(command, CancellationToken.None));
    }
}
