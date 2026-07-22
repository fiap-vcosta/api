using Application.UseCases.Administrativo.Servico.Commands.DeleteServico;
using Domain.Exceptions;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Administrativo.Servico.Commands.DeleteServico;

public class DeleteServicoCommandHandlerTests
{
    private readonly Mock<IServicoGateway> _mockGateway;
    private readonly DeleteServicoCommandHandler _handler;

    public DeleteServicoCommandHandlerTests()
    {
        _mockGateway = new Mock<IServicoGateway>();
        _handler = new DeleteServicoCommandHandler(_mockGateway.Object);
    }

    [Fact]
    public async Task Handle_DeletesServico_WhenServicoExists()
    {
        _mockGateway.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new ServicoAggregateRoot { Id = 1, Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true });
        _mockGateway.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        await _handler.Handle(new DeleteServicoCommand { Id = 1 }, CancellationToken.None);

        _mockGateway.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsDomainNotFoundException_WhenServicoDoesNotExist()
    {
        _mockGateway.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ServicoAggregateRoot?)null);

        await Assert.ThrowsAsync<DomainNotFoundException>(() => _handler.Handle(new DeleteServicoCommand { Id = 999 }, CancellationToken.None));
    }
}
