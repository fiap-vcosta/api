using MediatR;

namespace Application.Administrativo.Usuario.Commands.Login;

public class LoginCommand : IRequest<string>
{
    public string Login { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}