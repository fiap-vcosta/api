using Application.UseCases.OrdemServico.Commands.ConfirmarPagamentoOrdemServico;
using Application.Abstractions.Gateways;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.ValueObjects;
using Moq;

namespace UnitTests.Application.UseCases.OrdemServico.Commands.ConfirmarPagamentoOrdemServico;

public class ConfirmarPagamentoOrdemServicoCommandHandlerTests
{
    [Fact]
    public async Task Handle_ConfirmsPayment_WhenFinalized()
    {
        // Arrange
        var mockOrdemServicoGateway = new Mock<IOrdemServicoGateway>();

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

        mockOrdemServicoGateway.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ordemServico);
        mockOrdemServicoGateway.Setup(r => r.UpdateAsync(It.IsAny<OrdemServicoAggregateRoot>())).Returns(Task.CompletedTask);

        var handler = new ConfirmarPagamentoOrdemServicoCommandHandler(mockOrdemServicoGateway.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusOrdemServico.Entregue, result.Status);
    }
}
