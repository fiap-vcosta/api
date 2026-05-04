using Application.Commands;
using Application.Repositories;
using Application.Services;
using Domain;
using MediatR;

namespace Application.Handlers;

public class LoginCommandHandler(IUsuarioRepository usuarioRepository, IJwtService jwtService)
    : IRequestHandler<LoginCommand, string>
{
    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var usuario = await usuarioRepository.GetByLoginAndPasswordAsync(request.Login, request.Password);
        if (usuario == null)
        {
            throw new UnauthorizedAccessException("Invalid login or password");
        }

        return jwtService.GenerateToken(usuario.Login, usuario.TipoUsuario.ToString(), usuario.Id);
    }
}