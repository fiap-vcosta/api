using Api.Controllers.Auth;
using Api.Controllers.Auth.Login;
using Api.Presenters.Auth;
using Api.ViewModels.Auth;
using Application.UseCases.Administrativo.Usuario.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Api.Controllers.Auth;

public class AuthControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _controller = new AuthController(
            _mediatorMock.Object,
            new AuthPresenter(),
            new LoginRequestValidator());
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenRequestIsValid()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<IRequest<LoginResponse>>(), CancellationToken.None))
            .ReturnsAsync(new LoginResponse { Token = "token-value" });

        var request = new LoginRequest { Login = "admin", Password = "password" };

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var viewModel = Assert.IsType<LoginViewModel>(okResult.Value);
        Assert.Equal("token-value", viewModel.Token);
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenLoginOrPasswordIsMissing()
    {
        // Arrange
        var request = new LoginRequest { Login = "", Password = "" };

        // Act
        var result = await _controller.Login(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        _mediatorMock.Verify(m => m.Send(It.IsAny<IRequest<LoginResponse>>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task Login_ThrowsUnauthorizedAccessException_WhenCredentialsAreInvalid()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<IRequest<LoginResponse>>(), CancellationToken.None))
            .ThrowsAsync(new UnauthorizedAccessException());

        var request = new LoginRequest { Login = "admin", Password = "password" };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.Login(request));
    }
}
