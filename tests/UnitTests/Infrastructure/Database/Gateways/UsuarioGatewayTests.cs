using Domain.Administrativo.Entities;
using Infrastructure.Database;
using Infrastructure.Database.Gateways;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Infrastructure.Database.Gateways;

public class UsuarioGatewayTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UsuarioGateway _gateway;

    public UsuarioGatewayTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _gateway = new UsuarioGateway(_context);

        // Seed test data
        _context.Usuarios.AddRange(
            new UsuarioAggregateRoot { Id = 1, Login = "admin", Senha = PasswordHasher.HashPassword("admin"), TipoUsuario = TipoUsuario.Admin },
            new UsuarioAggregateRoot { Id = 2, Login = "atendente", Senha = PasswordHasher.HashPassword("atendente"), TipoUsuario = TipoUsuario.Atendente },
            new UsuarioAggregateRoot { Id = 3, Login = "mecanico", Senha = PasswordHasher.HashPassword("mecanico"), TipoUsuario = TipoUsuario.Mecanico }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenUserExists()
    {
        // Act
        var user = await _gateway.GetByIdAsync(1);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(1, user.Id);
        Assert.Equal("admin", user.Login);
        Assert.Equal(TipoUsuario.Admin, user.TipoUsuario);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Act
        var user = await _gateway.GetByIdAsync(999);

        // Assert
        Assert.Null(user);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        // Act
        var users = (await _gateway.GetAllAsync()).ToList();

        // Assert
        Assert.NotNull(users);
        Assert.Equal(3, users.Count);
        Assert.Contains(users, u => u.Login == "admin");
        Assert.Contains(users, u => u.Login == "atendente");
        Assert.Contains(users, u => u.Login == "mecanico");
    }

    [Fact]
    public async Task GetByLoginAndSenhaAsync_ReturnsUser_WhenCredentialsMatch()
    {
        // Act
        var user = await _gateway.GetByLoginAndSenhaAsync("admin", "admin");

        // Assert
        Assert.NotNull(user);
        Assert.Equal(1, user.Id);
        Assert.Equal("admin", user.Login);
        Assert.Equal(TipoUsuario.Admin, user.TipoUsuario);
    }

    [Fact]
    public async Task GetByLoginAndSenhaAsync_ReturnsNull_WhenSenhaDoesNotMatch()
    {
        // Act
        var user = await _gateway.GetByLoginAndSenhaAsync("admin", "wrong");

        // Assert
        Assert.Null(user);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _context.Dispose();
    }
}