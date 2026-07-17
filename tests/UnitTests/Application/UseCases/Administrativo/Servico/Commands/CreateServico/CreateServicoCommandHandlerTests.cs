using Application.UseCases.Administrativo.Servico.Commands.CreateServico;
using Domain.Exceptions;
using Application.UseCases.Administrativo.Servico.Responses;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Administrativo.Servico.Commands.CreateServico;

public class CreateServicoCommandHandlerTests
{
    private readonly Mock<IServicoGateway> _mockGateway;
    private readonly CreateServicoCommandHandler _handler;

    public CreateServicoCommandHandlerTests()
    {
        _mockGateway = new Mock<IServicoGateway>();
        _handler = new CreateServicoCommandHandler(_mockGateway.Object);
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

        _mockGateway.Setup(r => r.GetByCodigoAsync(command.Codigo))
            .ReturnsAsync((ServicoAggregateRoot?)null);

        _mockGateway.Setup(r => r.CreateAsync(It.IsAny<ServicoAggregateRoot>()))
            .Callback<ServicoAggregateRoot>(s => s.Id = 1)
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("OLE-001", result.Codigo);
        Assert.Equal("Serviço de Óleo", result.Nome);
        Assert.Equal(150.00m, result.PrecoPadrao);
        Assert.True(result.Ativo);
        _mockGateway.Verify(r => r.CreateAsync(It.IsAny<ServicoAggregateRoot>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsBusinessRuleException_WhenCodigoAlreadyExists()
    {
        var command = new CreateServicoCommand { Codigo = "OLE-001", Nome = "Serviço", PrecoPadrao = 150.00m, Ativo = true };
        _mockGateway.Setup(r => r.GetByCodigoAsync(command.Codigo))
            .ReturnsAsync(new ServicoAggregateRoot { Id = 2, Codigo = command.Codigo, Nome = "Outro", PrecoPadrao = 100.00m, Ativo = true });

        await Assert.ThrowsAsync<BusinessRuleException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
