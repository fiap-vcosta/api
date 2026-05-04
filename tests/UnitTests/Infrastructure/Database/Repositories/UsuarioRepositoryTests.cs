using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Infrastructure.Database.Repositories;

public class UsuarioRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UsuarioRepository _repository;

    public UsuarioRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new UsuarioRepository(_context);

        // Seed test data
        _context.Usuarios.AddRange(
            new Usuario { Id = 1, Login = "admin", Password = "hashed_admin", TipoUsuario = TipoUsuario.Admin },
            new Usuario { Id = 2, Login = "atendente", Password = "hashed_atendente", TipoUsuario = TipoUsuario.Atendente },
            new Usuario { Id = 3, Login = "mecanico", Password = "hashed_mecanico", TipoUsuario = TipoUsuario.Mecanico }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenUserExists()
    {
        // Act
        var user = await _repository.GetByIdAsync(1);

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
        var user = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(user);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        // Act
        var users = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, users.Count());
        Assert.Contains(users, u => u.Login == "admin");
        Assert.Contains(users, u => u.Login == "atendente");
        Assert.Contains(users, u => u.Login == "mecanico");
    }

    [Fact]
    public async Task GetByLoginAsync_ReturnsUser_WhenLoginExists()
    {
        // Act
        var user = await _repository.GetByLoginAsync("admin");

        // Assert
        Assert.NotNull(user);
        Assert.Equal(1, user.Id);
        Assert.Equal("admin", user.Login);
        Assert.Equal(TipoUsuario.Admin, user.TipoUsuario);
    }

    [Fact]
    public async Task GetByLoginAsync_ReturnsNull_WhenLoginDoesNotExist()
    {
        // Act
        var user = await _repository.GetByLoginAsync("nonexistent");

        // Assert
        Assert.Null(user);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}