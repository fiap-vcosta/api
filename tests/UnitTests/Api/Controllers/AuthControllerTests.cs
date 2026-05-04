using Application.Commands;
using Application.Validators;
using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Api.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_ReturnsOk_WhenRequestIsValid()
    {
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<IRequest<string>>(), default))
            .ReturnsAsync("token-value");

        var validator = new LoginRequestValidator();
        var controller = new AuthController(mediatorMock.Object, validator);

        var result = await controller.Login(new LoginRequest { Login = "admin", Password = "password" });

        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = okResult.Value!;
        var tokenProperty = value.GetType().GetProperty("Token");

        Assert.NotNull(tokenProperty);
        Assert.Equal("token-value", tokenProperty!.GetValue(value));
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenLoginOrPasswordIsMissing()
    {
        var mediatorMock = new Mock<IMediator>();
        var validator = new LoginRequestValidator();
        var controller = new AuthController(mediatorMock.Object, validator);

        var result = await controller.Login(new LoginRequest { Login = "", Password = "" });

        Assert.IsType<BadRequestObjectResult>(result);
        mediatorMock.Verify(m => m.Send(It.IsAny<IRequest<string>>(), default), Times.Never);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenMediatorThrowsUnauthorizedAccessException()
    {
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<IRequest<string>>(), default))
            .ThrowsAsync(new UnauthorizedAccessException());

        var validator = new LoginRequestValidator();
        var controller = new AuthController(mediatorMock.Object, validator);

        var result = await controller.Login(new LoginRequest { Login = "admin", Password = "password" });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
