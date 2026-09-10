using Api.Controllers.Cliente.SystemApi;
using Application.UseCases.Administrativo.Cliente.Queries.ConsultarClientePorDocumento;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Api.Controllers.Cliente.SystemApi;

public class ClienteSystemControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();

    [Fact]
    public async Task PorDocumento_ReturnsBadRequest_WhenDocumentoInvalid()
    {
        // Arrange
        var controller = new ClienteSystemController(_mediatorMock.Object);

        // Act
        var result = await controller.PorDocumento("123", CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<ConsultarClientePorDocumentoQuery>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task PorDocumento_ReturnsOk_WhenClienteExists()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ConsultarClientePorDocumentoQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var controller = new ClienteSystemController(_mediatorMock.Object);

        // Act
        var result = await controller.PorDocumento("11144477735", CancellationToken.None);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task PorDocumento_ReturnsNotFound_WhenClienteDoesNotExist()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ConsultarClientePorDocumentoQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var controller = new ClienteSystemController(_mediatorMock.Object);

        // Act
        var result = await controller.PorDocumento("11144477735", CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
