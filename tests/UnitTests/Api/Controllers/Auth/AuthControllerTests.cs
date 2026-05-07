using Api.Controllers.Auth;
using Api.Controllers.Auth.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Api.Controllers.Auth;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_ReturnsOk_WhenRequestIsValid()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<IRequest<string>>(), CancellationToken.None))
            .ReturnsAsync("token-value");

        var validator = new LoginRequestValidator();
        var controller = new AuthController(mediatorMock.Object, validator);
        var request = new LoginRequest { Login = "admin", Password = "password" };

        // Act
        var result = await controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = okResult.Value!;
        var tokenProperty = value.GetType().GetProperty("Token");

        Assert.NotNull(tokenProperty);
        Assert.Equal("token-value", tokenProperty.GetValue(value));
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenLoginOrPasswordIsMissing()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var validator = new LoginRequestValidator();
        var controller = new AuthController(mediatorMock.Object, validator);
        var request = new LoginRequest { Login = "", Password = "" };

        // Act
        var result = await controller.Login(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        mediatorMock.Verify(m => m.Send(It.IsAny<IRequest<string>>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenMediatorThrowsUnauthorizedAccessException()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<IRequest<string>>(), CancellationToken.None))
            .ThrowsAsync(new UnauthorizedAccessException());

        var validator = new LoginRequestValidator();
        var controller = new AuthController(mediatorMock.Object, validator);
        var request = new LoginRequest { Login = "admin", Password = "password" };

        // Act
        var result = await controller.Login(request);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
