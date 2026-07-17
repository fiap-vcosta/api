using Api.Presenters.Auth;
using Application.UseCases.Administrativo.Usuario.Responses;

namespace UnitTests.Api.Presenters.Auth;

public class AuthPresenterTests
{
    private readonly AuthPresenter _presenter = new();

    [Fact]
    public void Present_MapsTokenToViewModel()
    {
        // Arrange
        var response = new LoginResponse { Token = "jwt-token-value" };

        // Act
        var viewModel = _presenter.Present(response);

        // Assert
        Assert.Equal(response.Token, viewModel.Token);
    }
}
