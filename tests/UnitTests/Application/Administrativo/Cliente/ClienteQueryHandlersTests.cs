using Application.Administrativo.Cliente.Queries;
using Application.Administrativo.Cliente.Queries.Handlers;
using Domain.Administrativo.Entities;
using Domain.Administrativo.Repositories;
using Moq;

namespace UnitTests.Application.Administrativo.Cliente;

public class ClienteQueryHandlersTests
{
    private readonly Mock<IClienteRepository> _mockRepository = new();

    [Fact]
    public async Task GetClienteByIdQueryHandler_ReturnsCliente_WhenClienteExists()
    {
        // Arrange
        var cliente = new Domain.Administrativo.Entities.Cliente { Id = 1, Nome = "Cliente Test", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" };
        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(cliente);

        var handler = new GetClienteByIdQueryHandler(_mockRepository.Object);
        var query = new GetClienteByIdQuery { Id = 1 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Cliente Test", result.Nome);
        Assert.Equal(0, result.TipoDocumento);
    }

    [Fact]
    public async Task GetClienteByIdQueryHandler_ReturnsNull_WhenClienteDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Domain.Administrativo.Entities.Cliente?)null);

        var handler = new GetClienteByIdQueryHandler(_mockRepository.Object);
        var query = new GetClienteByIdQuery { Id = 999 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllClientesQueryHandler_ReturnsAllClientes()
    {
        // Arrange
        var clientes = new List<Domain.Administrativo.Entities.Cliente>
        {
            new() { Id = 1, Nome = "Cliente 1", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" },
            new() { Id = 2, Nome = "Cliente 2", TipoDocumento = TipoDocumento.Cnpj, Documento = "12345678901234" }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(clientes);

        var handler = new GetAllClientesQueryHandler(_mockRepository.Object);
        var query = new GetAllClientesQuery();

        // Act
        var result = (await handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Nome == "Cliente 1");
        Assert.Contains(result, c => c.Nome == "Cliente 2");
    }

    [Fact]
    public async Task GetAllClientesQueryHandler_ReturnsEmptyList_WhenNoClientesExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Domain.Administrativo.Entities.Cliente>());

        var handler = new GetAllClientesQueryHandler(_mockRepository.Object);
        var query = new GetAllClientesQuery();

        // Act
        var result = (await handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
