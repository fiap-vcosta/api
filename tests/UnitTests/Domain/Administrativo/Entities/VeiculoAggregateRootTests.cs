using Domain.Administrativo.Entities;

namespace UnitTests.Domain.Administrativo.Entities;

public class VeiculoAggregateRootTests
{
    [Fact]
    public void CreateVeiculoAggregateRoot_SetsPropertiesCorrectly()
    {
        // Arrange
        var veiculo = new VeiculoAggregateRoot
        {
            Id = 7,
            IdDono = 42,
            Placa = "ABC-1234",
            Modelo = "Civic",
            Marca = "Honda"
        };

        // Act & Assert
        Assert.Equal(7, veiculo.Id);
        Assert.Equal(42, veiculo.IdDono);
        Assert.Equal("ABC-1234", veiculo.Placa);
        Assert.Equal("Civic", veiculo.Modelo);
        Assert.Equal("Honda", veiculo.Marca);
    }
}
