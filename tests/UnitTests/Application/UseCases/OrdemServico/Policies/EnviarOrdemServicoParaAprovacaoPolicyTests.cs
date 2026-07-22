using Application.Abstractions.Events;
using Domain.Exceptions;
using Application.Abstractions.Services;
using Application.UseCases.OrdemServico.Policies;
using Domain.Administrativo.Entities;
using Application.Abstractions.Gateways;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Events;
using Domain.OrdemServico.ValueObjects;
using Moq;

namespace UnitTests.Application.UseCases.OrdemServico.Policies;

public class EnviarOrdemServicoParaAprovacaoPolicyTests
{
    [Fact]
    public async Task Handle_SendsEmailToCliente()
    {
        // Arrange
        var mockOrdemServicoGateway = new Mock<IOrdemServicoGateway>();
        var mockClienteGateway = new Mock<IClienteGateway>();
        var mockSmtpService = new Mock<ISmtpService>();

        var ordem = CriarOrdemBasica();
        ReflectSetId(ordem, 15);

        var cliente = new ClienteAggregateRoot
        {
            Id = 1,
            Nome = "Maria",
            Email = "maria@teste.com"
        };

        mockOrdemServicoGateway.Setup(r => r.GetByIdAsync(15)).ReturnsAsync(ordem);
        mockClienteGateway.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cliente);
        mockSmtpService.Setup(s => s.EnviarEmail(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        var policy = new EnviarOrdemServicoParaAprovacaoPolicy(
            mockOrdemServicoGateway.Object,
            mockClienteGateway.Object,
            mockSmtpService.Object);

        // Act
        await policy.Handle(new DomainEventNotification<DiagnosticoPreenchidoEvent>(new DiagnosticoPreenchidoEvent(15)), CancellationToken.None);

        // Assert
        mockSmtpService.Verify(
            s => s.EnviarEmail("maria@teste.com", It.Is<string>(c => c.Contains("15"))),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_WhenClienteNotFound()
    {
        // Arrange
        var mockOrdemServicoGateway = new Mock<IOrdemServicoGateway>();
        var mockClienteGateway = new Mock<IClienteGateway>();

        var ordem = CriarOrdemBasica();
        ReflectSetId(ordem, 15);

        mockOrdemServicoGateway.Setup(r => r.GetByIdAsync(15)).ReturnsAsync(ordem);
        mockClienteGateway.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ClienteAggregateRoot?)null);

        var policy = new EnviarOrdemServicoParaAprovacaoPolicy(
            mockOrdemServicoGateway.Object,
            mockClienteGateway.Object,
            new Mock<ISmtpService>().Object);

        // Act & Assert
        await Assert.ThrowsAsync<DomainNotFoundException>(() =>
            policy.Handle(new DomainEventNotification<DiagnosticoPreenchidoEvent>(new DiagnosticoPreenchidoEvent(15)), CancellationToken.None));
    }

    private static OrdemServicoAggregateRoot CriarOrdemBasica()
    {
        var cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" };
        var veiculo = new VeiculoOrdemServico { Placa = "XYZ-9876", Marca = "Toyota", Modelo = "Corolla" };
        return OrdemServicoAggregateRoot.Criar(cliente, veiculo);
    }

    private static void ReflectSetId(OrdemServicoAggregateRoot ordem, int id)
    {
        typeof(OrdemServicoAggregateRoot).GetProperty(nameof(OrdemServicoAggregateRoot.Id))!.SetValue(ordem, id);
    }
}
