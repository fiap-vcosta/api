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

    // Additional tests for uncovered branches and lines
    
    [Fact]
    public void AddServiceItem_WhenInDiagnosis_AddsServiceSuccessfully()
    {
        // Arrange
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        var ordem = CriarOrdem();
        ordem.EnviarParaDiagnostico();

        // Act
        ordem.AdicionarItemServico("Troca de pneus", 200m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>
        {
            new(1, 4m, new ItemEstoqueOrdemServico { Id = 100, Nome = "Pneu" })
        });

        // Assert
        Assert.Single(ordem.Servicos);
        Assert.Equal("Troca de pneus", ordem.Servicos.First().Nome);
    }

    [Fact]
    public void FinalizeDiagnosis_WhenNoServicesAdded_ThrowsInvalidOperationException()
    {
        // Arrange
        var ordem = CriarOrdem();
        ordem.EnviarParaDiagnostico();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => ordem.FinalizarDiagnostico());
    }

    [Fact]
    public void FinalizeDiagnosis_WithOnlyRejectedServices_SetsDeliveredStatus()
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
        ordem.RejeitarServicosSugeridos();

        // Act
        ordem.FinalizarDiagnostico();

        // Assert
        Assert.Equal(StatusOrdemServico.Entregue, ordem.Status);
        Assert.NotNull(ordem.EntregueEm);
    }

    [Fact]
    public void RejectSuggestedServices_WhenAwaitingApproval_RejetsAllSuggestedServicesAndReturnsToEmDiagnostico()
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

        // Act
        ordem.RejeitarServicosSugeridos();

        // Assert
        Assert.Equal(StatusOrdemServico.EmDiagnostico, ordem.Status);
        Assert.All(ordem.Servicos, s => Assert.Equal(StatusItemOrdemServico.Rejeitado, s.Status));
    }

    [Fact]
    public void ApproveAllSuggestedServices_WhenAwaitingApproval_ApprovesAllAndGoesToStockCheck()
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

        // Act
        ordem.AprovarServicosSugeridos();

        // Assert
        Assert.Equal(StatusOrdemServico.ChecandoEstoque, ordem.Status);
        Assert.NotNull(ordem.AprovadaEm);
        Assert.All(ordem.Servicos, s => Assert.Equal(StatusItemOrdemServico.Aprovado, s.Status));
    }

    [Fact]
    public void ApproveServicesPartially_WhenAwaitingApproval_ApprovesSelectedAndRejetsOthers()
    {
        // Arrange
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        var ordem = CriarOrdem();

        ordem.EnviarParaDiagnostico();
        ordem.AdicionarItemServico("Troca de pneus", 200m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>
        {
            new(1, 4m, new ItemEstoqueOrdemServico { Id = 100, Nome = "Pneu" })
        });
        ordem.AdicionarItemServico("Alinhamento", 150m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>());
        ordem.FinalizarDiagnostico();

        var idPrimeiroServico = ordem.Servicos.First().Id;

        // Act
        ordem.AprovarServicosParcialmente(new List<int> { idPrimeiroServico });

        // Assert
        Assert.Equal(StatusOrdemServico.EmDiagnostico, ordem.Status);
        var servicoAprovado = ordem.Servicos.First(s => s.Id == idPrimeiroServico);
        Assert.Equal(StatusItemOrdemServico.Aprovado, servicoAprovado.Status);
    }

    [Fact]
    public void ChecarItensNecessarios_WhenStockIsInsufficient_SetsAguardandoPeca()
    {
        // Arrange
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        var ordem = CriarOrdem();

        ordem.EnviarParaDiagnostico();
        ordem.AdicionarItemServico("Troca de pneus", 200m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>
        {
            new(1, 10m, new ItemEstoqueOrdemServico { Id = 100, Nome = "Pneu" })
        });
        ordem.FinalizarDiagnostico();
        ordem.AprovarServicosSugeridos();

        var saldos = new Dictionary<int, decimal> { [100] = 5m }; // Insufficient stock

        // Act
        ordem.ChecarItensNecessarios(saldos);

        // Assert
        Assert.Equal(StatusOrdemServico.AguardandoPeca, ordem.Status);
    }

    [Fact]
    public void ChecarItensNecessarios_WhenStatusIsAguardandoPeca_ChecksAgain()
    {
        // Arrange
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        var ordem = CriarOrdem();

        ordem.EnviarParaDiagnostico();
        ordem.AdicionarItemServico("Troca de pneus", 200m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>
        {
            new(1, 10m, new ItemEstoqueOrdemServico { Id = 100, Nome = "Pneu" })
        });
        ordem.FinalizarDiagnostico();
        ordem.AprovarServicosSugeridos();

        // First check with insufficient stock
        var saldosInsuficientes = new Dictionary<int, decimal> { [100] = 5m };
        ordem.ChecarItensNecessarios(saldosInsuficientes);
        Assert.Equal(StatusOrdemServico.AguardandoPeca, ordem.Status);

        // Act - Check again with sufficient stock
        var saldosSuficientes = new Dictionary<int, decimal> { [100] = 15m };
        ordem.ChecarItensNecessarios(saldosSuficientes);

        // Assert
        Assert.Equal(StatusOrdemServico.LiberadaParaExecucao, ordem.Status);
    }

    [Fact]
    public void TravarItensNecessarios_LockAllRequiredItems()
    {
        // Arrange
        var ordem = CriarOrdemComServicoAprovado();

        // Act
        ordem.TravarItensNecessarios();

        // Assert
        Assert.All(ordem.ItensNecessariosParaExecucao, item => 
            Assert.Equal(StatusItemEstoque.EstoqueTravado, item.Status));
    }

    [Fact]
    public void ConfirmExecution_WhenAllServicesCompleted_SetsFinalizedStatus()
    {
        // Arrange
        var ordem = CriarOrdemComServicoAprovado();
        ordem.ChecarItensNecessarios(new Dictionary<int, decimal> { [100] = 5m });
        ordem.TravarItensNecessarios();

        var servicoId = ordem.Servicos.First().Id;

        // Act
        ordem.ConfirmarExecucao(new List<ServicoExecutado>
        {
            new() { IdServico = servicoId, IniciadoEm = DateTime.UtcNow.AddHours(-1), FinalizadoEm = DateTime.UtcNow }
        });

        // Assert
        Assert.Equal(StatusOrdemServico.Finalizada, ordem.Status);
        Assert.All(ordem.Servicos, s => Assert.Equal(StatusItemOrdemServico.Concluido, s.Status));
    }

    [Fact]
    public void ConfirmExecution_WhenPartiallyCompleted_StaysInExecution()
    {
        // Arrange
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        var ordem = CriarOrdem();

        ordem.EnviarParaDiagnostico();
        ordem.AdicionarItemServico("Troca de pneus", 200m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>
        {
            new(1, 4m, new ItemEstoqueOrdemServico { Id = 100, Nome = "Pneu" })
        });
        ordem.AdicionarItemServico("Alinhamento", 150m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>());
        ordem.FinalizarDiagnostico();
        ordem.AprovarServicosSugeridos();
        ordem.ChecarItensNecessarios(new Dictionary<int, decimal> { [100] = 5m });
        ordem.TravarItensNecessarios();

        var firstServicoId = ordem.Servicos.First().Id;

        // Act
        ordem.ConfirmarExecucao(new List<ServicoExecutado>
        {
            new() { IdServico = firstServicoId, IniciadoEm = DateTime.UtcNow.AddHours(-1), FinalizadoEm = DateTime.UtcNow }
        });

        // Assert
        Assert.Equal(StatusOrdemServico.EmExecucao, ordem.Status);
    }

    [Fact]
    public void ConfirmExecution_WhenEmExecucao_ConfirmsMoreServices()
    {
        // Arrange
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        var ordem = CriarOrdem();

        ordem.EnviarParaDiagnostico();
        ordem.AdicionarItemServico("Serviço 1", 200m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>());
        ordem.AdicionarItemServico("Serviço 2", 150m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>());
        ordem.FinalizarDiagnostico();
        ordem.AprovarServicosSugeridos();
        ordem.ChecarItensNecessarios(new Dictionary<int, decimal>());

        var servicos = ordem.Servicos.ToList();
        servicos[0].GetType().GetProperty("Id")?.SetValue(servicos[0], 1);
        servicos[1].GetType().GetProperty("Id")?.SetValue(servicos[1], 2);

        var servicoIds = ordem.Servicos.Select(s => s.Id).ToList();

        // First confirm
        ordem.ConfirmarExecucao(new List<ServicoExecutado>
        {
            new() { IdServico = servicoIds[0], IniciadoEm = DateTime.UtcNow.AddHours(-2), FinalizadoEm = DateTime.UtcNow.AddHours(-1) }
        });
        Assert.Equal(StatusOrdemServico.EmExecucao, ordem.Status);

        // Act - Confirm the second one
        ordem.ConfirmarExecucao(new List<ServicoExecutado>
        {
            new() { IdServico = servicoIds[1], IniciadoEm = DateTime.UtcNow.AddHours(-1), FinalizadoEm = DateTime.UtcNow }
        });

        // Assert
        Assert.Equal(StatusOrdemServico.Finalizada, ordem.Status);
    }

    [Fact]
    public void ConfirmPayment_WhenNotFinalized_ThrowsInvalidOperationException()
    {
        // Arrange
        var ordem = CriarOrdem();
        ordem.EnviarParaDiagnostico();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => ordem.ConfirmarPagamento());
    }

    [Fact]
    public void ValorTotal_ExcludesRejectedServices()
    {
        // Arrange
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        var ordem = CriarOrdem();

        ordem.EnviarParaDiagnostico();
        ordem.AdicionarItemServico("Serviço Aprovado", 200m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>());
        ordem.AdicionarItemServico("Serviço Rejeitado", 150m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>());
        ordem.FinalizarDiagnostico();
        ordem.RejeitarServicosSugeridos();

        // Act
        var valorTotal = ordem.ValorTotal;

        // Assert
        Assert.Equal(0m, valorTotal); // All services were rejected
    }

    [Fact]
    public void ValorTotal_IncludesApprovedAndSuggestedButExcludesRejected()
    {
        // Arrange
        var servicoCatalogo = new ServicoCatalogo() { Id = 1, Nome = "Serviço", Codigo = "SVR-001" };
        var ordem = CriarOrdem();

        ordem.EnviarParaDiagnostico();
        ordem.AdicionarItemServico("Serviço 1", 100m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>());
        ordem.AdicionarItemServico("Serviço 2", 200m, servicoCatalogo, new List<ItemNecessario.CriarItemNecessarioParams>());
        ordem.FinalizarDiagnostico();

        // Act
        var valorTotal = ordem.ValorTotal;

        // Assert
        Assert.Equal(300m, valorTotal); // Both are still pending approval (Sugerido status counts)
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
