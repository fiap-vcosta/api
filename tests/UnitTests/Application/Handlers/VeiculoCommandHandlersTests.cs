using Application.Veiculo.Commands;
using Application.Veiculo.Commands.Handlers;
using Domain.Entities;
using Domain.Repositories;
using Moq;

namespace UnitTests.Application.Handlers;

public class CreateVeiculoCommandHandlerTests
{
    private readonly Mock<IClienteRepository> _mockClienteRepository;
    private readonly Mock<IVeiculoRepository> _mockVeiculoRepository;
    private readonly CreateVeiculoCommandHandler _handler;

    public CreateVeiculoCommandHandlerTests()
    {
        _mockClienteRepository = new Mock<IClienteRepository>();
        _mockVeiculoRepository = new Mock<IVeiculoRepository>();
        _handler = new CreateVeiculoCommandHandler(_mockClienteRepository.Object, _mockVeiculoRepository.Object);
    }

    [Fact]
    public async Task Handle_CreatesVeiculo_WhenCommandIsValid()
    {
        var command = new CreateVeiculoCommand
        {
            Placa = "ABC-1D23",
            DonoId = 1,
            Modelo = "Gol",
            Marca = "Volkswagen"
        };

        _mockClienteRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Cliente { Id = 1, Nome = "Cliente Teste", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" });

        _mockVeiculoRepository.Setup(r => r.GetByPlacaAsync(command.Placa))
            .ReturnsAsync((Veiculo?)null);

        _mockVeiculoRepository.Setup(r => r.CreateAsync(It.IsAny<Veiculo>()))
            .Callback<Veiculo>(v => v.Id = 1)
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("ABC-1D23", result.Placa);
        Assert.Equal(1, result.DonoId);
        Assert.Equal("Gol", result.Modelo);
        Assert.Equal("Volkswagen", result.Marca);
        _mockVeiculoRepository.Verify(r => r.CreateAsync(It.IsAny<Veiculo>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsKeyNotFoundException_WhenDonoDoesNotExist()
    {
        var command = new CreateVeiculoCommand { Placa = "ABC-1D23", DonoId = 999, Modelo = "Gol", Marca = "Volkswagen" };
        _mockClienteRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Cliente?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ThrowsInvalidOperationException_WhenPlacaAlreadyExists()
    {
        var command = new CreateVeiculoCommand { Placa = "ABC-1D23", DonoId = 1, Modelo = "Gol", Marca = "Volkswagen" };
        _mockClienteRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Cliente { Id = 1, Nome = "Cliente Teste", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" });
        _mockVeiculoRepository.Setup(r => r.GetByPlacaAsync(command.Placa))
            .ReturnsAsync(new Veiculo { Id = 2, Placa = command.Placa, DonoId = 1, Modelo = "Uno", Marca = "Fiat" });

        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}

public class UpdateVeiculoCommandHandlerTests
{
    private readonly Mock<IClienteRepository> _mockClienteRepository;
    private readonly Mock<IVeiculoRepository> _mockVeiculoRepository;
    private readonly UpdateVeiculoCommandHandler _handler;

    public UpdateVeiculoCommandHandlerTests()
    {
        _mockClienteRepository = new Mock<IClienteRepository>();
        _mockVeiculoRepository = new Mock<IVeiculoRepository>();
        _handler = new UpdateVeiculoCommandHandler(_mockClienteRepository.Object, _mockVeiculoRepository.Object);
    }

    [Fact]
    public async Task Handle_UpdatesVeiculo_WhenVeiculoExists()
    {
        var command = new UpdateVeiculoCommand
        {
            Id = 1,
            Placa = "DEF-2G34",
            DonoId = 1,
            Modelo = "Polo",
            Marca = "Volkswagen"
        };

        _mockVeiculoRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Veiculo { Id = 1, Placa = "ABC-1D23", DonoId = 1, Modelo = "Gol", Marca = "Volkswagen" });

        _mockClienteRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Cliente { Id = 1, Nome = "Cliente Teste", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" });

        _mockVeiculoRepository.Setup(r => r.GetByPlacaAsync(command.Placa))
            .ReturnsAsync((Veiculo?)null);

        _mockVeiculoRepository.Setup(r => r.UpdateAsync(It.IsAny<Veiculo>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("DEF-2G34", result.Placa);
        Assert.Equal("Polo", result.Modelo);
        _mockVeiculoRepository.Verify(r => r.UpdateAsync(It.IsAny<Veiculo>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsKeyNotFoundException_WhenVeiculoDoesNotExist()
    {
        var command = new UpdateVeiculoCommand { Id = 999, Placa = "DEF-2G34", DonoId = 1, Modelo = "Polo", Marca = "Volkswagen" };
        _mockVeiculoRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Veiculo?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ThrowsInvalidOperationException_WhenAnotherVeiculoHasSamePlaca()
    {
        var command = new UpdateVeiculoCommand { Id = 1, Placa = "DEF-2G34", DonoId = 1, Modelo = "Polo", Marca = "Volkswagen" };

        _mockVeiculoRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Veiculo { Id = 1, Placa = "ABC-1D23", DonoId = 1, Modelo = "Gol", Marca = "Volkswagen" });

        _mockClienteRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Cliente { Id = 1, Nome = "Cliente Teste", TipoDocumento = TipoDocumento.Cpf, Documento = "11144477735" });

        _mockVeiculoRepository.Setup(r => r.GetByPlacaAsync(command.Placa))
            .ReturnsAsync(new Veiculo { Id = 2, Placa = command.Placa, DonoId = 1, Modelo = "Uno", Marca = "Fiat" });

        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}

public class DeleteVeiculoCommandHandlerTests
{
    private readonly Mock<IVeiculoRepository> _mockVeiculoRepository;
    private readonly DeleteVeiculoCommandHandler _handler;

    public DeleteVeiculoCommandHandlerTests()
    {
        _mockVeiculoRepository = new Mock<IVeiculoRepository>();
        _handler = new DeleteVeiculoCommandHandler(_mockVeiculoRepository.Object);
    }

    [Fact]
    public async Task Handle_DeletesVeiculo_WhenVeiculoExists()
    {
        _mockVeiculoRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Veiculo { Id = 1, Placa = "ABC-1D23", DonoId = 1, Modelo = "Gol", Marca = "Volkswagen" });
        _mockVeiculoRepository.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        await _handler.Handle(new DeleteVeiculoCommand { Id = 1 }, CancellationToken.None);

        _mockVeiculoRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsKeyNotFoundException_WhenVeiculoDoesNotExist()
    {
        _mockVeiculoRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Veiculo?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(new DeleteVeiculoCommand { Id = 999 }, CancellationToken.None));
    }
}
