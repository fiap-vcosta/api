using Application.Abstractions.Services;
using Application.Abstractions.Gateways;
using Application.UseCases.Administrativo.Usuario.Responses;
using MediatR;

namespace Application.UseCases.Administrativo.Usuario.Commands.Login;

public class LoginCommandHandler(IUsuarioGateway usuarioGateway, IJwtService jwtService)
    : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var usuario = await usuarioGateway.GetByLoginAndSenhaAsync(request.Login, request.Senha);
        
        var token = usuario == null
            ? throw new UnauthorizedAccessException("Login ou senha inválidos.")
            : jwtService.GenerateToken(usuario.Login, usuario.TipoUsuario.ToString(), usuario.Id);

        return new LoginResponse { Token = token };
    }
}