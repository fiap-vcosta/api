using Application.Servico.Commands;
using Application.Servico.Commands.Handlers;
using Domain.Entities;
using Domain.Repositories;
using Moq;

namespace UnitTests.Application.Handlers;

public class CreateServicoCommandHandlerTests
{
    private readonly Mock<IServicoRepository> _mockRepository;
    private readonly CreateServicoCommandHandler _handler;

    public CreateServicoCommandHandlerTests()
    {
        _mockRepository = new Mock<IServicoRepository>();
        _handler = new CreateServicoCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_CreatesServico_WhenCommandIsValid()
    {
        var command = new CreateServicoCommand
        {
            Codigo = "OLE-001",
            Nome = "Serviço de Óleo",
            PrecoPadrao = 150.00m,
            Ativo = true
        };

        _mockRepository.Setup(r => r.GetByCodigoAsync(command.Codigo))
            .ReturnsAsync((Servico?)null);

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Servico>()))
            .Callback<Servico>(s => s.Id = 1)
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("OLE-001", result.Codigo);
        Assert.Equal("Serviço de Óleo", result.Nome);
        Assert.Equal(150.00m, result.PrecoPadrao);
        Assert.True(result.Ativo);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Servico>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsInvalidOperationException_WhenCodigoAlreadyExists()
    {
        var command = new CreateServicoCommand { Codigo = "OLE-001", Nome = "Serviço", PrecoPadrao = 150.00m, Ativo = true };
        _mockRepository.Setup(r => r.GetByCodigoAsync(command.Codigo))
            .ReturnsAsync(new Servico { Id = 2, Codigo = command.Codigo, Nome = "Outro", PrecoPadrao = 100.00m, Ativo = true });

        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}

public class UpdateServicoCommandHandlerTests
{
    private readonly Mock<IServicoRepository> _mockRepository;
    private readonly UpdateServicoCommandHandler _handler;

    public UpdateServicoCommandHandlerTests()
    {
        _mockRepository = new Mock<IServicoRepository>();
        _handler = new UpdateServicoCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_UpdatesServico_WhenServicoExists()
    {
        var command = new UpdateServicoCommand
        {
            Id = 1,
            Codigo = "FRE-001",
            Nome = "Serviço de Freio",
            PrecoPadrao = 250.00m,
            Ativo = true
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Servico { Id = 1, Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true });

        _mockRepository.Setup(r => r.GetByCodigoAsync(command.Codigo))
            .ReturnsAsync((Servico?)null);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Servico>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("FRE-001", result.Codigo);
        Assert.Equal("Serviço de Freio", result.Nome);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Servico>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsKeyNotFoundException_WhenServicoDoesNotExist()
    {
        var command = new UpdateServicoCommand { Id = 999, Codigo = "FRE-001", Nome = "Freio", PrecoPadrao = 250.00m, Ativo = true };
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Servico?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}

public class DeleteServicoCommandHandlerTests
{
    private readonly Mock<IServicoRepository> _mockRepository;
    private readonly DeleteServicoCommandHandler _handler;

    public DeleteServicoCommandHandlerTests()
    {
        _mockRepository = new Mock<IServicoRepository>();
        _handler = new DeleteServicoCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_DeletesServico_WhenServicoExists()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Servico { Id = 1, Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true });
        _mockRepository.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        await _handler.Handle(new DeleteServicoCommand { Id = 1 }, CancellationToken.None);

        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsKeyNotFoundException_WhenServicoDoesNotExist()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Servico?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(new DeleteServicoCommand { Id = 999 }, CancellationToken.None));
    }
}
