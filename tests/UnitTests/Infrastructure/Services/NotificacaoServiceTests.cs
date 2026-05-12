using Domain.Administrativo.Entities;
using Infrastructure.Services;

namespace UnitTests.Infrastructure.Services;

public class NotificacaoServiceTests
{
    [Fact]
    public async Task NotificarUsuariosPorTipo_CompletesSuccessfully()
    {
        // Arrange
        var service = new NotificacaoService();
        var tipoUsuario = TipoUsuario.Mecanico;
        var conteudo = "Teste de notificação";

        // Act && Assert method completes without exception
        await service.NotificarUsuariosPorTipo(tipoUsuario, conteudo);
    }
}
