using Application.Abstractions.Services;
using Domain.Administrativo.Repositories;
using MediatR;

namespace Application.Administrativo.Usuario.Commands.Login;

public class LoginCommandHandler(IUsuarioRepository usuarioRepository, IJwtService jwtService)
    : IRequestHandler<LoginCommand, string>
{
    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var usuario = await usuarioRepository.GetByLoginAndPasswordAsync(request.Login, request.Password);
        
        return usuario == null
            ? throw new UnauthorizedAccessException("Invalid login or password")
            : jwtService.GenerateToken(usuario.Login, usuario.TipoUsuario.ToString(), usuario.Id);
    }
}