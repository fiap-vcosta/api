using Application.Abstractions.Services;
using Application.Core.OrdemServico.Commands.AlocarEstoqueOrdemServico;
using Application.Core.OrdemServico.Policies;
using Domain.Administrativo.Entities;
using Domain.Administrativo.Repositories;
using Domain.Estoque.Entities;
using Domain.Estoque.Events;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Events;
using Domain.OrdemServico.Repositories;
using Domain.OrdemServico.ValueObjects;
using MediatR;
using Moq;

namespace UnitTests.Application.Core.OrdemServico;

public class OrdemServicoPoliciesTests
{
    [Fact]
    public async Task ChecarEstoqueOrdemServicoPolicy_SendsAlocarEstoqueCommand()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        mockMediator
            .Setup(m => m.Send(It.IsAny<AlocarEstoqueOrdemServicoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);
        var policy = new ChecarEstoqueOrdemServicoPolicy(mockMediator.Object);

        // Act
        await policy.Handle(new OrdemServicoAprovadaEvent(42), CancellationToken.None);

        // Assert
        mockMediator.Verify(
            m => m.Send(It.Is<AlocarEstoqueOrdemServicoCommand>(c => c.idOrdemServico == 42), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ChecarFilaDeEsperaOrdemServicoPolicy_AllocatesOrdersWaitingForPart()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        var mockMediator = new Mock<IMediator>();

        var ordem1 = CriarOrdemBasica();
        ReflectSetId(ordem1, 10);
        var ordem2 = CriarOrdemBasica();
        ReflectSetId(ordem2, 20);

        var item = new ItemEstoqueAggregateRoot { Id = 100, Nome = "Pneu", Saldo = 50m };

        mockOrdemServicoRepository
            .Setup(r => r.GetAguardandoPecaPorItemEstoqueAsync(100))
            .ReturnsAsync(new List<OrdemServicoAggregateRoot> { ordem1, ordem2 });
        mockMediator
            .Setup(m => m.Send(It.IsAny<AlocarEstoqueOrdemServicoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var policy = new ChecarFilaDeEsperaOrdemServicoPolicy(mockOrdemServicoRepository.Object, mockMediator.Object);

        // Act
        await policy.Handle(new ChegadaDeItensRegistradaEvent(item), CancellationToken.None);

        // Assert
        mockMediator.Verify(
            m => m.Send(It.Is<AlocarEstoqueOrdemServicoCommand>(c => c.idOrdemServico == 10), It.IsAny<CancellationToken>()),
            Times.Once);
        mockMediator.Verify(
            m => m.Send(It.Is<AlocarEstoqueOrdemServicoCommand>(c => c.idOrdemServico == 20), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task EnviarOrdemServicoParaDiagnosticoPolicy_MovesToDiagnosticoAndNotifies()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        var mockNotificacaoService = new Mock<INotificacaoService>();
        var mockMediator = new Mock<IMediator>();

        var ordem = CriarOrdemBasica();
        ReflectSetId(ordem, 7);

        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(ordem);
        mockOrdemServicoRepository.Setup(r => r.UpdateAsync(It.IsAny<OrdemServicoAggregateRoot>())).Returns(Task.CompletedTask);
        mockNotificacaoService
            .Setup(n => n.NotificarUsuariosPorTipo(TipoUsuario.Mecanico, It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        mockMediator
            .Setup(m => m.Publish(It.IsAny<OrdemServicoRecebidaDiagnosticoEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var policy = new EnviarOrdemServicoParaDiagnosticoPolicy(
            mockOrdemServicoRepository.Object,
            mockNotificacaoService.Object,
            mockMediator.Object);

        // Act
        await policy.Handle(new OrdemServicoCriadaEvent(7), CancellationToken.None);

        // Assert
        Assert.Equal(StatusOrdemServico.EmDiagnostico, ordem.Status);
        mockOrdemServicoRepository.Verify(r => r.UpdateAsync(ordem), Times.Once);
        mockNotificacaoService.Verify(
            n => n.NotificarUsuariosPorTipo(TipoUsuario.Mecanico, It.Is<string>(s => s.Contains("7"))),
            Times.Once);
        mockMediator.Verify(
            m => m.Publish(It.Is<OrdemServicoRecebidaDiagnosticoEvent>(e => e.IdOrdemServico == 7), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task EnviarOrdemServicoParaDiagnosticoPolicy_Throws_WhenOrdemNotFound()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((OrdemServicoAggregateRoot?)null);

        var policy = new EnviarOrdemServicoParaDiagnosticoPolicy(
            mockOrdemServicoRepository.Object,
            new Mock<INotificacaoService>().Object,
            new Mock<IMediator>().Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            policy.Handle(new OrdemServicoCriadaEvent(999), CancellationToken.None));
    }

    [Fact]
    public async Task EnviarOrdemServicoParaAprovacaoPolicy_SendsEmailToCliente()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        var mockClienteRepository = new Mock<IClienteRepository>();
        var mockSmtpService = new Mock<ISMTPService>();

        var ordem = CriarOrdemBasica();
        ReflectSetId(ordem, 15);

        var cliente = new ClienteAggregateRoot
        {
            Id = 1,
            Nome = "Maria",
            Email = "maria@teste.com"
        };

        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(15)).ReturnsAsync(ordem);
        mockClienteRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cliente);
        mockSmtpService.Setup(s => s.EnviarEmail(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        var policy = new EnviarOrdemServicoParaAprovacaoPolicy(
            mockOrdemServicoRepository.Object,
            mockClienteRepository.Object,
            mockSmtpService.Object);

        // Act
        await policy.Handle(new DiagnosticoPreenchidoEvent(15), CancellationToken.None);

        // Assert
        mockSmtpService.Verify(
            s => s.EnviarEmail("maria@teste.com", It.Is<string>(c => c.Contains("15"))),
            Times.Once);
    }

    [Fact]
    public async Task EnviarOrdemServicoParaAprovacaoPolicy_Throws_WhenClienteNotFound()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        var mockClienteRepository = new Mock<IClienteRepository>();

        var ordem = CriarOrdemBasica();
        ReflectSetId(ordem, 15);

        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(15)).ReturnsAsync(ordem);
        mockClienteRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ClienteAggregateRoot?)null);

        var policy = new EnviarOrdemServicoParaAprovacaoPolicy(
            mockOrdemServicoRepository.Object,
            mockClienteRepository.Object,
            new Mock<ISMTPService>().Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            policy.Handle(new DiagnosticoPreenchidoEvent(15), CancellationToken.None));
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
