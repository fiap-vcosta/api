using Domain.OrdemServico.Entities;
using Domain.OrdemServico.ValueObjects;

namespace UnitTests.Domain.OrdemServico.Entities;

public class ServicoTests
{
    [Fact]
    public void CreateServico_SetsSuggestedStatusAndProperties()
    {
        // Arrange & Act
        var servico = Servico.Criar("Troca de pastilhas", 200m);

        // Assert
        Assert.Equal(StatusItemOrdemServico.Sugerido, servico.Status);
        Assert.Equal("Troca de pastilhas", servico.Nome);
        Assert.Equal(200m, servico.ValorCobrado);
        Assert.Empty(servico.ItensNecessarios);
    }

    [Fact]
    public void Approve_SetsApprovedStatus()
    {
        // Arrange
        var servico = Servico.Criar("Troca de pastilhas", 200m);

        // Act
        servico.Aprovar();

        // Assert
        Assert.Equal(StatusItemOrdemServico.Aprovado, servico.Status);
        Assert.NotEqual(default, servico.AprovadoEm);
    }

    [Fact]
    public void Approve_WhenStatusIsNotSuggested_ThrowsInvalidOperationException()
    {
        // Arrange
        var servico = Servico.Criar("Troca de pastilhas", 200m);
        servico.Aprovar();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => servico.Aprovar());
    }

    [Fact]
    public void Reject_SetsRejectedStatus()
    {
        // Arrange
        var servico = Servico.Criar("Diagnóstico", 100m);

        // Act
        servico.Rejeitar();

        // Assert
        Assert.Equal(StatusItemOrdemServico.Rejeitado, servico.Status);
        Assert.NotEqual(default, servico.RejeitadoEm);
    }

    [Fact]
    public void Reject_WhenStatusIsNotSuggested_ThrowsInvalidOperationException()
    {
        // Arrange
        var servico = Servico.Criar("Diagnóstico", 100m);
        servico.Rejeitar();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => servico.Rejeitar());
    }

    [Fact]
    public void ConfirmConclusion_WhenApproved_SetsConcludedStatusAndUpdatesItems()
    {
        // Arrange
        var servico = Servico.Criar("Troca de óleo", 150m);
        var itemEstoque = new ItemEstoqueOrdemServico { Id = 21, Nome = "Óleo" };
        servico.AdicionarItemNecessario(new ItemNecessario.CriarItemNecessarioParams(1, 2m, itemEstoque));
        servico.Aprovar();
        servico.ItensNecessarios.First().ChecarEstoque(3m);
        servico.ItensNecessarios.First().TravarEstoque();

        // Act
        var iniciado = DateTime.UtcNow.AddMinutes(-30);
        var finalizado = DateTime.UtcNow;
        servico.ConfirmarConclusao(iniciado, finalizado);

        // Assert
        Assert.Equal(StatusItemOrdemServico.Concluido, servico.Status);
        Assert.Equal(iniciado, servico.ExecucaoIniciadaEm);
        Assert.Equal(finalizado, servico.ExecucaoFinalizadaEm);
        Assert.All(servico.ItensNecessarios, item => Assert.Equal(StatusItemEstoque.Utilizado, item.Status));
    }

    [Fact]
    public void ConfirmConclusion_WhenNotApproved_ThrowsInvalidOperationException()
    {
        // Arrange
        var servico = Servico.Criar("Troca de óleo", 150m);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => servico.ConfirmarConclusao(DateTime.UtcNow, DateTime.UtcNow));
    }
}
