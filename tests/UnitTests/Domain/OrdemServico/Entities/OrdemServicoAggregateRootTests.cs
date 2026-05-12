using Domain.OrdemServico.Entities;
using Domain.OrdemServico.ValueObjects;

namespace UnitTests.Domain.OrdemServico.Entities;

public class OrdemServicoAggregateRootTests
{
    [Fact]
    public void CreateOrderService_SetsReceivedStatusAndCustomerAndVehicle()
    {
        // Arrange
        var cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" };
        var veiculo = new VeiculoOrdemServico { Placa = "XYZ-9876", Marca = "Toyota", Modelo = "Corolla" };

        // Act
        var ordem = OrdemServicoAggregateRoot.Criar(cliente, veiculo);

        // Assert
        Assert.Equal(StatusOrdemServico.Recebida, ordem.Status);
        Assert.Equal(cliente, ordem.Cliente);
        Assert.Equal(veiculo, ordem.Veiculo);
        Assert.True((DateTime.UtcNow - ordem.RecebidaEm).TotalSeconds < 5);
    }

    [Fact]
    public void SendToDiagnostics_WhenReceived_ChangesStatusToDiagnostics()
    {
        // Arrange
        var ordem = CriarOrdem();

        // Act
        ordem.EnviarParaDiagnostico();

        // Assert
        Assert.Equal(StatusOrdemServico.EmDiagnostico, ordem.Status);
    }

    [Fact]
    public void SendToDiagnostics_WhenNotReceived_ThrowsInvalidOperationException()
    {
        // Arrange
        var ordem = CriarOrdem();
        ordem.EnviarParaDiagnostico();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => ordem.EnviarParaDiagnostico());
    }

    [Fact]
    public void Discard_WhenStatusIsReceived_SetsDiscardedStatus()
    {
        // Arrange
        var ordem = CriarOrdem();

        // Act
        ordem.Descartar();

        // Assert
        Assert.Equal(StatusOrdemServico.Descartada, ordem.Status);
        Assert.NotNull(ordem.DescartadaEm);
    }

    [Fact]
    public void Discard_WhenStatusIsInDiagnosis_SetsDiscardedStatus()
    {
        // Arrange
        var ordem = CriarOrdem();
        ordem.EnviarParaDiagnostico();

        // Act
        ordem.Descartar();

        // Assert
        Assert.Equal(StatusOrdemServico.Descartada, ordem.Status);
        Assert.NotNull(ordem.DescartadaEm);
    }

    [Fact]
    public void Discard_WhenStatusIsNotDiscardable_ThrowsInvalidOperationException()
    {
        // Arrange
        var ordem = CriarOrdem();
        ordem.Descartar();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => ordem.Descartar());
    }

    [Fact]
    public void AddServiceItem_WhenNotInDiagnosis_ThrowsInvalidOperationException()
    {
        // Arrange
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        var ordem = CriarOrdem();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => ordem.AdicionarItemServico("Troca de pneus", 200m, servicoCatalogo,
            new List<ItemNecessario.CriarItemNecessarioParams>()));
    }

    [Fact]
    public void FinalizeDiagnosis_WithSuggestedService_SetsAwaitingApproval()
    {
        // Arrange
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        var ordem = CriarOrdem();

        ordem.EnviarParaDiagnostico();
        ordem.AdicionarItemServico("Troca de pneus", 200m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>
        {
            new(1, 4m, new ItemEstoqueOrdemServico { Id = 100, Nome = "Pneu" })
        });

        // Act
        ordem.FinalizarDiagnostico();

        // Assert
        Assert.Equal(StatusOrdemServico.AguardandoAprovacao, ordem.Status);
    }

    [Fact]
    public void FinalizeDiagnosis_WithApprovedService_GoesToStockCheck()
    {
        // Arrange
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        var ordem = CriarOrdem();

        ordem.EnviarParaDiagnostico();
        ordem.AdicionarItemServico("Troca de pneus", 200m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>
        {
            new(1, 4m, new ItemEstoqueOrdemServico { Id = 100, Nome = "Pneu" })
        });
        ordem.FinalizarDiagnostico();
        ordem.AprovarServicosSugeridos();

        // Act
        var status = ordem.Status;

        // Assert
        Assert.Equal(StatusOrdemServico.ChecandoEstoque, status);
    }

    [Fact]
    public void RejectSuggestedServices_WhenNotAwaitingApproval_ThrowsInvalidOperationException()
    {
        // Arrange
        var ordem = CriarOrdem();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => ordem.RejeitarServicosSugeridos());
    }

    [Fact]
    public void ApproveServicesPartially_WhenNotAwaitingApproval_ThrowsInvalidOperationException()
    {
        // Arrange
        var ordem = CriarOrdem();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => ordem.AprovarServicosParcialmente(new List<int> { 1 }));
    }

    [Fact]
    public void CheckRequiredItems_WhenStockIsSufficient_SetsLiberatedToExecute()
    {
        // Arrange
        var ordem = CriarOrdemComServicoAprovado();
        var saldos = new Dictionary<int, decimal> { [100] = 5m };

        // Act
        ordem.ChecarItensNecessarios(saldos);

        // Assert
        Assert.Equal(StatusOrdemServico.LiberadaParaExecucao, ordem.Status);
    }

    [Fact]
    public void CheckRequiredItems_WhenStatusIsInvalid_ThrowsInvalidOperationException()
    {
        // Arrange
        var ordem = CriarOrdem();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => ordem.ChecarItensNecessarios(new Dictionary<int, decimal> { [100] = 5m }));
    }

    [Fact]
    public void ConfirmExecution_WhenOrderIsNotReady_ThrowsInvalidOperationException()
    {
        // Arrange
        var ordem = CriarOrdem();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => ordem.ConfirmarExecucao(new List<ServicoExecutado> { new() { IdServico = 1, IniciadoEm = DateTime.UtcNow.AddHours(-1), FinalizadoEm = DateTime.UtcNow } }));
    }

    [Fact]
    public void ConfirmPayment_SetsDeliveredStatus()
    {
        // Arrange
        var ordem = CriarOrdem();

        // Act
        ordem.ConfirmarPagamento();

        // Assert
        Assert.Equal(StatusOrdemServico.Entregue, ordem.Status);
    }

    private static OrdemServicoAggregateRoot CriarOrdem()
    {
        var cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" };
        var veiculo = new VeiculoOrdemServico { Placa = "XYZ-9876", Marca = "Toyota", Modelo = "Corolla" };
        return OrdemServicoAggregateRoot.Criar(cliente, veiculo);
    }

    private static OrdemServicoAggregateRoot CriarOrdemComServicoAprovado()
    {
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        var ordem = CriarOrdem();

        ordem.EnviarParaDiagnostico();
        ordem.AdicionarItemServico("Troca de pneus", 200m, servicoCatalogo,new List<ItemNecessario.CriarItemNecessarioParams>
        {
            new(1, 4m, new ItemEstoqueOrdemServico { Id = 100, Nome = "Pneu" })
        });
        ordem.FinalizarDiagnostico();
        ordem.AprovarServicosSugeridos();
        return ordem;
    }
}
