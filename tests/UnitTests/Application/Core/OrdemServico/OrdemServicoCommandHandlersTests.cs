using Application.Core.OrdemServico.Commands.CriarOrdemServico;
using Application.Core.OrdemServico.Commands.AdicionarItemOrdemServico;
using Application.Core.OrdemServico.Commands.DescartarOrdemServico;
using Application.Core.OrdemServico.Commands.AprovarOrdemServico;
using Application.Core.OrdemServico.Commands.AprovarServicosParcialmente;
using Application.Core.OrdemServico.Commands.FinalizarDiagnostico;
using Application.Core.OrdemServico.Commands.ConfirmarExecucaoOrdemServico;
using Application.Core.OrdemServico.Commands.ConfirmarPagamentoOrdemServico;
using Application.Core.OrdemServico.Commands.RejeitarOrdemServico;
using Application.Core.OrdemServico.Commands.AlocarEstoqueOrdemServico;
using Domain.Administrativo.Entities;
using Domain.Administrativo.Repositories;
using Domain.Estoque.Entities;
using Domain.Estoque.Repositories;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.Events;
using Domain.OrdemServico.Repositories;
using Domain.OrdemServico.ValueObjects;
using MediatR;
using Moq;

namespace UnitTests.Application.Core.OrdemServico;

public class OrdemServicoCommandHandlersTests
{
    [Fact]
    public async Task CriarOrdemServicoCommandHandler_CreatesOrdemServico_WhenCommandIsValid()
    {
        // Arrange
        var mockVeiculoRepository = new Mock<IVeiculoRepository>();
        var mockClienteRepository = new Mock<IClienteRepository>();
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        var mockMediator = new Mock<IMediator>();

        var cliente = new ClienteAggregateRoot { Id = 1, Nome = "João", Email = "joao@test.com", TipoDocumento = TipoDocumento.Cpf, Documento = "12345678901" };
        var veiculo = new VeiculoAggregateRoot { Id = 1, Placa = "ABC-1234", Marca = "Toyota", Modelo = "Corolla", IdDono = 1 };
        
        var command = new CriarOrdemServicoCommand { IdVeiculo = 1 };

        mockVeiculoRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(veiculo);
        mockClienteRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cliente);
        mockOrdemServicoRepository.Setup(r => r.CriarAsync(It.IsAny<OrdemServicoAggregateRoot>())).Returns(Task.CompletedTask);
        mockMediator.Setup(m => m.Publish(It.IsAny<OrdemServicoCriadaEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new CriarOrdemServicoCommandHandler(
            mockVeiculoRepository.Object,
            mockClienteRepository.Object,
            mockOrdemServicoRepository.Object,
            mockMediator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cliente.Nome, result.Cliente.Nome);
        Assert.Equal(veiculo.Placa, result.Veiculo.Placa);
        mockOrdemServicoRepository.Verify(r => r.CriarAsync(It.IsAny<OrdemServicoAggregateRoot>()), Times.Once);
        mockMediator.Verify(m => m.Publish(It.IsAny<OrdemServicoCriadaEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DescartarOrdemServicoCommandHandler_DiscardsOrdemServico_WhenCommandIsValid()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        var mockMediator = new Mock<IMediator>();

        var cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" };
        var veiculo = new VeiculoOrdemServico { Placa = "XYZ-9876", Marca = "Toyota", Modelo = "Corolla" };
        var ordemServico = OrdemServicoAggregateRoot.Criar(cliente, veiculo);

        var command = new DescartarOrdemServicoCommand { IdOrdemServico = 1 };

        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ordemServico);
        mockOrdemServicoRepository.Setup(r => r.UpdateAsync(It.IsAny<OrdemServicoAggregateRoot>())).Returns(Task.CompletedTask);
        mockMediator.Setup(m => m.Publish(It.IsAny<OrdemServicoDescartadaEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new DescartarOrdemServicoCommandHandler(mockOrdemServicoRepository.Object, mockMediator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusOrdemServico.Descartada, result.Status);
        mockOrdemServicoRepository.Verify(r => r.UpdateAsync(It.IsAny<OrdemServicoAggregateRoot>()), Times.Once);
    }

    [Fact]
    public async Task AprovarOrdemServicoCommandHandler_ApprovesOrdemServico_WhenCommandIsValid()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        var mockMediator = new Mock<IMediator>();

        var cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" };
        var veiculo = new VeiculoOrdemServico { Placa = "XYZ-9876", Marca = "Toyota", Modelo = "Corolla" };
        var ordemServico = OrdemServicoAggregateRoot.Criar(cliente, veiculo);
        ordemServico.EnviarParaDiagnostico();
        
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        ordemServico.AdicionarItemServico("Troca", 100m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>());
        ordemServico.FinalizarDiagnostico();

        var command = new AprovarOrdemServicoCommand { IdOrdemServico = 1 };

        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ordemServico);
        mockOrdemServicoRepository.Setup(r => r.UpdateAsync(It.IsAny<OrdemServicoAggregateRoot>())).Returns(Task.CompletedTask);
        mockMediator.Setup(m => m.Publish(It.IsAny<OrdemServicoAprovadaEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new AprovarOrdemServicoCommandHandler(mockOrdemServicoRepository.Object, mockMediator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusOrdemServico.ChecandoEstoque, result.Status);
    }

    [Fact]
    public async Task FinalizarDiagnosticoCommandHandler_FinalizesWithSuggestedServices()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        var mockMediator = new Mock<IMediator>();

        var cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" };
        var veiculo = new VeiculoOrdemServico { Placa = "XYZ-9876", Marca = "Toyota", Modelo = "Corolla" };
        var ordemServico = OrdemServicoAggregateRoot.Criar(cliente, veiculo);
        ordemServico.EnviarParaDiagnostico();
        
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        ordemServico.AdicionarItemServico("Troca", 100m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>());

        var command = new FinalizarDiagnosticoCommand { IdOrdemServico = 1 };

        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ordemServico);
        mockOrdemServicoRepository.Setup(r => r.UpdateAsync(It.IsAny<OrdemServicoAggregateRoot>())).Returns(Task.CompletedTask);
        mockMediator.Setup(m => m.Publish(It.IsAny<DiagnosticoPreenchidoEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new FinalizarDiagnosticoCommandHandler(mockOrdemServicoRepository.Object, mockMediator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusOrdemServico.AguardandoAprovacao, result.Status);
    }

    [Fact]
    public async Task ConfirmarExecucaoOrdemServicoCommandHandler_ConfirmsExecution_WhenAllServicesComplete()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        var mockMediator = new Mock<IMediator>();

        var cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" };
        var veiculo = new VeiculoOrdemServico { Placa = "XYZ-9876", Marca = "Toyota", Modelo = "Corolla" };
        var ordemServico = OrdemServicoAggregateRoot.Criar(cliente, veiculo);
        ordemServico.EnviarParaDiagnostico();
        
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        ordemServico.AdicionarItemServico("Troca", 100m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>());
        ordemServico.FinalizarDiagnostico();
        ordemServico.AprovarServicosSugeridos();
        ordemServico.ChecarItensNecessarios(new Dictionary<int, decimal>());

        var servicoId = ordemServico.Servicos.First().Id;

        var command = new ConfirmarExecucaoOrdemServicoCommand
        {
            IdOrdemServico = 1,
            ServicoExecutados = new List<ServicoExecutado>
            {
                new() { IdServico = servicoId, IniciadoEm = DateTime.UtcNow.AddHours(-1), FinalizadoEm = DateTime.UtcNow }
            }
        };

        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ordemServico);
        mockOrdemServicoRepository.Setup(r => r.UpdateAsync(It.IsAny<OrdemServicoAggregateRoot>())).Returns(Task.CompletedTask);
        mockMediator.Setup(m => m.Send(It.IsAny<IRequest<Unit>>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(Unit.Value));

        var handler = new ConfirmarExecucaoOrdemServicoCommandHandler(mockOrdemServicoRepository.Object, mockMediator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusOrdemServico.Finalizada, result.Status);
    }

    [Fact]
    public async Task ConfirmarPagamentoOrdemServicoCommandHandler_ConfirmsPayment_WhenFinalized()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();

        var cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" };
        var veiculo = new VeiculoOrdemServico { Placa = "XYZ-9876", Marca = "Toyota", Modelo = "Corolla" };
        var ordemServico = OrdemServicoAggregateRoot.Criar(cliente, veiculo);
        ordemServico.EnviarParaDiagnostico();
        
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        ordemServico.AdicionarItemServico("Troca", 100m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>());
        ordemServico.FinalizarDiagnostico();
        ordemServico.AprovarServicosSugeridos();
        ordemServico.ChecarItensNecessarios(new Dictionary<int, decimal>());
        
        var servicoId = ordemServico.Servicos.First().Id;
        ordemServico.ConfirmarExecucao(new List<ServicoExecutado>
        {
            new() { IdServico = servicoId, IniciadoEm = DateTime.UtcNow.AddHours(-1), FinalizadoEm = DateTime.UtcNow }
        });

        var command = new ConfirmarPagamentoOrdemServicoCommand { IdOrdemServico = 1 };

        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ordemServico);
        mockOrdemServicoRepository.Setup(r => r.UpdateAsync(It.IsAny<OrdemServicoAggregateRoot>())).Returns(Task.CompletedTask);

        var handler = new ConfirmarPagamentoOrdemServicoCommandHandler(mockOrdemServicoRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusOrdemServico.Entregue, result.Status);
    }

    [Fact]
    public async Task RejeitarOrdemServicoCommandHandler_RejectsOrdemServico_WhenCommandIsValid()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();

        var cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" };
        var veiculo = new VeiculoOrdemServico { Placa = "XYZ-9876", Marca = "Toyota", Modelo = "Corolla" };
        var ordemServico = OrdemServicoAggregateRoot.Criar(cliente, veiculo);
        ordemServico.EnviarParaDiagnostico();
        
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        ordemServico.AdicionarItemServico("Troca", 100m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>());
        ordemServico.FinalizarDiagnostico();

        var command = new RejeitarOrdemServicoCommand { IdOrdemServico = 1 };

        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ordemServico);
        mockOrdemServicoRepository.Setup(r => r.UpdateAsync(It.IsAny<OrdemServicoAggregateRoot>())).Returns(Task.CompletedTask);

        var handler = new RejeitarOrdemServicoCommandHandler(mockOrdemServicoRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusOrdemServico.EmDiagnostico, result.Status);
    }

    [Fact]
    public async Task CommandHandlers_ThrowKeyNotFoundException_WhenOrdemServicoNotFound()
    {
        // Arrange
        var mockOrdemServicoRepository = new Mock<IOrdemServicoRepository>();
        mockOrdemServicoRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((OrdemServicoAggregateRoot?)null);

        // Test DescartarOrdemServicoCommandHandler
        var descartarHandler = new DescartarOrdemServicoCommandHandler(mockOrdemServicoRepository.Object, new Mock<IMediator>().Object);
        var descartarCommand = new DescartarOrdemServicoCommand { IdOrdemServico = 999 };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => descartarHandler.Handle(descartarCommand, CancellationToken.None));
    }
}
