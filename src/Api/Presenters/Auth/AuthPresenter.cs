using Api.ViewModels.Auth;
using Application.UseCases.Administrativo.Usuario.Responses;

namespace Api.Presenters.Auth;

public class AuthPresenter
{
    public LoginViewModel Present(LoginResponse response)
    {
        return new LoginViewModel
        {
            Token = response.Token
        };
    }
}
