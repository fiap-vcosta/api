using Domain.Administrativo.Entities;

namespace UnitTests.Domain.Administrativo.Entities;

public class ServicoAggregateRootTests
{
    [Fact]
    public void CreateServicoAggregateRoot_SetsDefaultValues()
    {
        // Arrange
        var servico = new ServicoAggregateRoot
        {
            Id = 10,
            Codigo = "SRV-01",
            Nome = "Troca de óleo",
            PrecoPadrao = 120.5m,
            Ativo = true
        };

        // Act & Assert
        Assert.Equal(10, servico.Id);
        Assert.Equal("SRV-01", servico.Codigo);
        Assert.Equal("Troca de óleo", servico.Nome);
        Assert.Equal(120.5m, servico.PrecoPadrao);
        Assert.True(servico.Ativo);
        Assert.Empty(servico.ItensNecessarios);
    }
}
