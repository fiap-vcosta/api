using Application.UseCases.Administrativo.Servico.Commands.UpdateServico;
using Domain.Exceptions;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Administrativo.Servico.Commands.UpdateServico;

public class UpdateServicoCommandHandlerTests
{
    private readonly Mock<IServicoGateway> _mockGateway;
    private readonly UpdateServicoCommandHandler _handler;

    public UpdateServicoCommandHandlerTests()
    {
        _mockGateway = new Mock<IServicoGateway>();
        _handler = new UpdateServicoCommandHandler(_mockGateway.Object);
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

        _mockGateway.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new ServicoAggregateRoot { Id = 1, Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true });

        _mockGateway.Setup(r => r.GetByCodigoAsync(command.Codigo))
            .ReturnsAsync((ServicoAggregateRoot?)null);

        _mockGateway.Setup(r => r.UpdateAsync(It.IsAny<ServicoAggregateRoot>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("FRE-001", result.Codigo);
        Assert.Equal("Serviço de Freio", result.Nome);
        _mockGateway.Verify(r => r.UpdateAsync(It.IsAny<ServicoAggregateRoot>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsDomainNotFoundException_WhenServicoDoesNotExist()
    {
        var command = new UpdateServicoCommand { Id = 999, Codigo = "FRE-001", Nome = "Freio", PrecoPadrao = 250.00m, Ativo = true };
        _mockGateway.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ServicoAggregateRoot?)null);

        await Assert.ThrowsAsync<DomainNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
