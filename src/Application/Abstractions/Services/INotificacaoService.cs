using Domain.Administrativo.Entities;

namespace Application.Abstractions.Services;

public interface INotificacaoService
{
    public Task NotificarUsuariosPorTipo(TipoUsuario tipoUsuario, string conteudo);
}
