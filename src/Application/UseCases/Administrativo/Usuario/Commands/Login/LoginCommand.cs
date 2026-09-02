using MediatR;
using Application.UseCases.Administrativo.Usuario.Responses;

namespace Application.UseCases.Administrativo.Usuario.Commands.Login;

public class LoginCommand : IRequest<LoginResponse>
{
    public string Login { get; init; } = string.Empty;
    public string Senha { get; init; } = string.Empty;
}