using Api.Controllers.Cliente.Internal;
using Api.Presenters.Cliente;
using Application.UseCases.Administrativo.Cliente.Queries.ConsultarClientePorDocumento;
using Application.UseCases.Administrativo.Cliente.Responses;
using Domain.Administrativo.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Api.Controllers.Cliente.Internal;

public class ClienteInternalControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly ConsultarClientePorDocumentoPresenter _presenter = new();

    [Fact]
    public async Task PorDocumento_ReturnsBadRequest_WhenDocumentoEmpty()
    {
        // Arrange
        var controller = new ClienteInternalController(_mediatorMock.Object, _presenter);

        // Act
        var result = await controller.PorDocumento("  ", CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        _mediatorMock.Verify(m => m.Send(It.IsAny<ConsultarClientePorDocumentoQuery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task PorDocumento_ReturnsOk_WhenQuerySucceeds()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ConsultarClientePorDocumentoQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConsultarClientePorDocumentoResponse
            {
                Existe = true,
                Documento = "11144477735",
                TipoDocumento = TipoDocumento.Cpf
            });
        var controller = new ClienteInternalController(_mediatorMock.Object, _presenter);

        // Act
        var result = await controller.PorDocumento("11144477735", CancellationToken.None);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }
}
