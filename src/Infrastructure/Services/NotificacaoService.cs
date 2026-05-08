using Application.Abstractions.Services;
using Domain.Administrativo.Entities;

namespace Infrastructure.Services;

public class NotificacaoService : INotificacaoService
{
    public Task NotificarUsuariosPorTipo(TipoUsuario tipoUsuario, string conteudo)
    {
        Console.WriteLine($"[Notificacao]<{tipoUsuario.ToString()}>: {conteudo}");
        return Task.CompletedTask;
    }
}
