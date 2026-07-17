using Application.Abstractions.Services;
using Domain.Exceptions;
using Application.UseCases.Pagamento.Commands.NotificarClienteSobreConclusao;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Moq;

namespace UnitTests.Application.UseCases.Pagamento.Commands.NotificarClienteSobreConclusao;

public class NotificarClienteSobreConclusaoCommandHandlerTests
{
    [Fact]
    public async Task Handle_Throws_WhenClienteNotFound()
    {
        // Arrange
        var mockClienteGateway = new Mock<IClienteGateway>();
        mockClienteGateway.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ClienteAggregateRoot?)null);
        var handler = new NotificarClienteSobreConclusaoCommandHandler(
            mockClienteGateway.Object,
            new Mock<ISMTPService>().Object);

        // Act / Assert
        await Assert.ThrowsAsync<DomainNotFoundException>(() =>
            handler.Handle(new NotificarClienteSobreConclusaoCommand { IdCliente = 999, IdOrdemServico = 1 }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_SendsEmail_WhenClienteExists()
    {
        // Arrange
        var mockClienteGateway = new Mock<IClienteGateway>();
        var mockSmtp = new Mock<ISMTPService>();
        mockClienteGateway.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new ClienteAggregateRoot
        {
            Id = 1,
            Nome = "Maria",
            Email = "maria@teste.com"
        });
        mockSmtp.Setup(s => s.EnviarEmail(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        var handler = new NotificarClienteSobreConclusaoCommandHandler(mockClienteGateway.Object, mockSmtp.Object);

        // Act
        await handler.Handle(new NotificarClienteSobreConclusaoCommand { IdCliente = 1, IdOrdemServico = 42 }, CancellationToken.None);

        // Assert
        mockSmtp.Verify(
            s => s.EnviarEmail("maria@teste.com", It.Is<string>(c => c.Contains("42"))),
            Times.Once);
    }
}
