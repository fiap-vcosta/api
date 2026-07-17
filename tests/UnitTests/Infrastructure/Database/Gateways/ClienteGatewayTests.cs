using Domain.Administrativo.Entities;
using Infrastructure.Database;
using Infrastructure.Database.Gateways;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Infrastructure.Database.Gateways;

public class ClienteGatewayTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ClienteGateway _gateway;

    public ClienteGatewayTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _gateway = new ClienteGateway(_context);

        // Seed test data
        _context.Clientes.AddRange(
            new ClienteAggregateRoot { Id = 1, Nome = "Cliente 1", TipoDocumento = TipoDocumento.Cpf, Documento = "12345678901" },
            new ClienteAggregateRoot { Id = 2, Nome = "Cliente 2", TipoDocumento = TipoDocumento.Cnpj, Documento = "12345678901234" }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCliente_WhenClienteExists()
    {
        // Act
        var cliente = await _gateway.GetByIdAsync(1);

        // Assert
        Assert.NotNull(cliente);
        Assert.Equal(1, cliente.Id);
        Assert.Equal("Cliente 1", cliente.Nome);
        Assert.Equal(TipoDocumento.Cpf, cliente.TipoDocumento);
        Assert.Equal("12345678901", cliente.Documento);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenClienteDoesNotExist()
    {
        // Act
        var cliente = await _gateway.GetByIdAsync(999);

        // Assert
        Assert.Null(cliente);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllClientes()
    {
        // Act
        var clientes = (await _gateway.GetAllAsync()).ToList();

        // Assert
        Assert.NotNull(clientes);
        Assert.Equal(2, clientes.Count);
        Assert.Contains(clientes, c => c.Nome == "Cliente 1");
        Assert.Contains(clientes, c => c.Nome == "Cliente 2");
    }

    [Fact]
    public async Task GetByDocumentoAsync_ReturnsCliente_WhenDocumentoExists()
    {
        // Act
        var cliente = await _gateway.GetByDocumentoAsync("12345678901");

        // Assert
        Assert.NotNull(cliente);
        Assert.Equal("Cliente 1", cliente.Nome);
    }

    [Fact]
    public async Task GetByDocumentoAsync_ReturnsNull_WhenDocumentoDoesNotExist()
    {
        // Act
        var cliente = await _gateway.GetByDocumentoAsync("99999999999");

        // Assert
        Assert.Null(cliente);
    }

    [Fact]
    public async Task CreateAsync_AddsCliente_ToDatabase()
    {
        // Arrange
        var newCliente = new ClienteAggregateRoot { Nome = "New Cliente", TipoDocumento = TipoDocumento.Cpf, Documento = "98765432101" };

        // Act
        await _gateway.CreateAsync(newCliente);

        // Assert
        var created = await _gateway.GetByDocumentoAsync("98765432101");
        Assert.NotNull(created);
        Assert.Equal("New Cliente", created.Nome);
    }

    [Fact]
    public async Task UpdateAsync_ModifiesCliente()
    {
        // Arrange
        var cliente = await _gateway.GetByIdAsync(1);
        Assert.NotNull(cliente);
        cliente.Nome = "Updated Cliente 1";

        // Act
        await _gateway.UpdateAsync(cliente);

        // Assert
        var updated = await _gateway.GetByIdAsync(1);
        Assert.NotNull(updated);
        Assert.Equal("Updated Cliente 1", updated.Nome);
    }

    [Fact]
    public async Task DeleteAsync_RemovesCliente()
    {
        // Act
        await _gateway.DeleteAsync(1);

        // Assert
        var deleted = await _gateway.GetByIdAsync(1);
        Assert.Null(deleted);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _context.Dispose();
    }
}
