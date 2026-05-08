using Application.Administrativo.Cliente.Commands;
using Application.Administrativo.Cliente.Commands.CreateCliente;
using Application.Administrativo.Cliente.Commands.DeleteCliente;
using Application.Administrativo.Cliente.Commands.UpdateCliente;
using Domain.Administrativo.Entities;
using Domain.Administrativo.Repositories;
using Moq;

namespace UnitTests.Application.Administrativo.Cliente;

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

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Domain.Administrativo.Entities.ClienteAggregateRoot>()))
            .Callback<Domain.Administrativo.Entities.ClienteAggregateRoot>(c => c.Id = 1)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Cliente Teste", result.Nome);
        Assert.Equal(TipoDocumento.Cpf, result.TipoDocumento);
        Assert.Equal("11144477735", result.Documento);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Domain.Administrativo.Entities.ClienteAggregateRoot>()), Times.Once);
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

        var existingCliente = new Domain.Administrativo.Entities.ClienteAggregateRoot { Id = 1, Nome = "Old Name", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" };
        
        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingCliente);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Domain.Administrativo.Entities.ClienteAggregateRoot>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Updated Cliente", result.Nome);
        Assert.Equal(TipoDocumento.Cnpj, result.TipoDocumento);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Domain.Administrativo.Entities.ClienteAggregateRoot>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsKeyNotFoundException_WhenClienteDoesNotExist()
    {
        // Arrange
        var command = new UpdateClienteCommand { Id = 999, Nome = "Test", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" };

        _mockRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Domain.Administrativo.Entities.ClienteAggregateRoot?)null);

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
        var existingCliente = new Domain.Administrativo.Entities.ClienteAggregateRoot { Id = 1, Nome = "Cliente to Delete", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" };

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
            .ReturnsAsync((Domain.Administrativo.Entities.ClienteAggregateRoot?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
