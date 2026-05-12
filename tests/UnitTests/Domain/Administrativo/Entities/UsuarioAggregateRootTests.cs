using Domain.Administrativo.Entities;

namespace UnitTests.Domain.Administrativo.Entities;

public class UsuarioAggregateRootTests
{
    [Fact]
    public void CreateUsuarioAggregateRoot_SetsPropertiesCorrectly()
    {
        // Arrange
        var usuario = new UsuarioAggregateRoot
        {
            Id = 99,
            Login = "usuario.teste",
            Password = "senha123",
            TipoUsuario = TipoUsuario.Cliente
        };

        // Act & Assert
        Assert.Equal(99, usuario.Id);
        Assert.Equal("usuario.teste", usuario.Login);
        Assert.Equal("senha123", usuario.Password);
        Assert.Equal(TipoUsuario.Cliente, usuario.TipoUsuario);
    }
}
