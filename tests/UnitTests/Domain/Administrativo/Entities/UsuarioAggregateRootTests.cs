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
            Senha = "senha123",
            TipoUsuario = TipoUsuario.Cliente
        };

        // Act & Assert
        Assert.Equal(99, usuario.Id);
        Assert.Equal("usuario.teste", usuario.Login);
        Assert.Equal("senha123", usuario.Senha);
        Assert.Equal(TipoUsuario.Cliente, usuario.TipoUsuario);
    }
}
