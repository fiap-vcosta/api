using Application.Cliente.Commands;
using Application.Cliente.Commands.Handlers;
using Domain.Entities;
using Domain.Repositories;
using Moq;

namespace UnitTests.Application.Handlers;

public class CreateClienteCommandHandlerTests
{
    private readonly Mock<IClienteRepository> _mockRepository;
    private readonly CreateClienteCommandHandler _handler;

    public CreateClienteCommandHandlerTests()
    {
        _mockRepository = new Mock<IClienteRepository>();
        _handler = new CreateClienteCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_CreatesCliente_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateClienteCommand
        {
            Nome = "Cliente Teste",
            TipoDocumento = TipoDocumento.Cpf,
            Documento = "11144477735"
        };

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Cliente>()))
            .Callback<Cliente>(c => c.Id = 1)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Cliente Teste", result.Nome);
        Assert.Equal(0, result.TipoDocumento); // CPF = 0
        Assert.Equal("11144477735", result.Documento);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Cliente>()), Times.Once);
    }
}

public class UpdateClienteCommandHandlerTests
{
    private readonly Mock<IClienteRepository> _mockRepository;
    private readonly UpdateClienteCommandHandler _handler;

    public UpdateClienteCommandHandlerTests()
    {
        _mockRepository = new Mock<IClienteRepository>();
        _handler = new UpdateClienteCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_UpdatesCliente_WhenClienteExists()
    {
        // Arrange
        var command = new UpdateClienteCommand
        {
            Id = 1,
            Nome = "Updated Cliente",
            TipoDocumento = TipoDocumento.Cnpj,
            Documento = "12345678901234"
        };

        var existingCliente = new Cliente { Id = 1, Nome = "Old Name", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" };
        
        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingCliente);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Cliente>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Updated Cliente", result.Nome);
        Assert.Equal(1, result.TipoDocumento); // CNPJ = 1
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Cliente>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsKeyNotFoundException_WhenClienteDoesNotExist()
    {
        // Arrange
        var command = new UpdateClienteCommand { Id = 999, Nome = "Test", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" };

        _mockRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Cliente?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}

public class DeleteClienteCommandHandlerTests
{
    private readonly Mock<IClienteRepository> _mockRepository;
    private readonly DeleteClienteCommandHandler _handler;

    public DeleteClienteCommandHandlerTests()
    {
        _mockRepository = new Mock<IClienteRepository>();
        _handler = new DeleteClienteCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_DeletesCliente_WhenClienteExists()
    {
        // Arrange
        var command = new DeleteClienteCommand { Id = 1 };
        var existingCliente = new Cliente { Id = 1, Nome = "Cliente to Delete", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" };

        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingCliente);

        _mockRepository.Setup(r => r.DeleteAsync(1))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsKeyNotFoundException_WhenClienteDoesNotExist()
    {
        // Arrange
        var command = new DeleteClienteCommand { Id = 999 };

        _mockRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Cliente?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
