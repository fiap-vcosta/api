namespace Api.Controllers.Auth.Login;

public class LoginRequest
{
    public string Login { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
