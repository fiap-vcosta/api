using MediatR;

namespace Application.Usuario.Commands;

public class LoginCommand : IRequest<string>
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}