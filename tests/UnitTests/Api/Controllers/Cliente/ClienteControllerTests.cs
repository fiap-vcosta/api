using Api.Contracts.Validation;
using Api.Controllers.Cliente;
using Api.Controllers.Cliente.CreateCliente;
using Api.Controllers.Cliente.UpdateCliente;
using Application.Administrativo.Cliente.Commands;
using Application.Administrativo.Cliente.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Api.Controllers.Cliente;

public class ClienteControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IValidator<CreateClienteRequest>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateClienteRequest>> _updateValidatorMock;
    private readonly ClienteController _controller;

    public ClienteControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _createValidatorMock = new Mock<IValidator<CreateClienteRequest>>();
        _updateValidatorMock = new Mock<IValidator<UpdateClienteRequest>>();
        _controller = new ClienteController(_mediatorMock.Object, _createValidatorMock.Object, _updateValidatorMock.Object);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = new CreateClienteRequest { Nome = "", TipoDocumento = 0, Documento = "" };
        var validationResult = new ValidationResult();
        validationResult.Errors.Add("Nome is required");
        
        _createValidatorMock.Setup(v => v.Validate(It.IsAny<CreateClienteRequest>()))
            .Returns(validationResult);

        // Act
        var result = await _controller.Create(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateClienteRequest { Nome = "Cliente Test", TipoDocumento = 0, Documento = "11144477735" };
        var response = new ClienteResponse { Id = 1, Nome = "Cliente Test", TipoDocumento = 0, Documento = "11144477735" };

        _createValidatorMock.Setup(v => v.Validate(It.IsAny<CreateClienteRequest>()))
            .Returns(new ValidationResult());

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateClienteCommand>(), CancellationToken.None))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ClienteController.GetById), createdResult.ActionName);
        Assert.Equal(response, createdResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenClienteExists()
    {
        // Arrange
        var response = new ClienteResponse { Id = 1, Nome = "Cliente Test", TipoDocumento = 0, Documento = "11144477735" };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetClienteByIdQuery>(), CancellationToken.None))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenClienteDoesNotExist()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetClienteByIdQuery>(), CancellationToken.None))
            .ReturnsAsync((ClienteResponse?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithListOfClientes()
    {
        // Arrange
        var clientes = new List<ClienteResponse>
        {
            new() { Id = 1, Nome = "Cliente 1", TipoDocumento = 0, Documento = "11144477735" },
            new() { Id = 2, Nome = "Cliente 2", TipoDocumento = 1, Documento = "12345678901234" }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllClientesQuery>(), CancellationToken.None))
            .ReturnsAsync(clientes);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(clientes, okResult.Value);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenRequestIsValid()
    {
        // Arrange
        var request = new UpdateClienteRequest { Nome = "Updated", TipoDocumento = 0, Documento = "11144477735" };
        var response = new ClienteResponse { Id = 1, Nome = "Updated", TipoDocumento = 0, Documento = "11144477735" };

        _updateValidatorMock.Setup(v => v.Validate(It.IsAny<UpdateClienteRequest>()))
            .Returns(new ValidationResult());

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetClienteByIdQuery>(), CancellationToken.None))
            .ReturnsAsync(response);

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateClienteCommand>(), CancellationToken.None))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Update(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenDocumentoIsNull()
    {
        // Arrange
        var request = new UpdateClienteRequest { Nome = "Updated" };
        var savedResponse = new ClienteResponse { Id = 1, Nome = "Cliente Test", TipoDocumento = 0, Documento = "11144477735" };
        var updateResponse = new ClienteResponse { Id = 1, Nome = "Updated", TipoDocumento = 0, Documento = "11144477735" };

        _updateValidatorMock.Setup(v => v.Validate(It.IsAny<UpdateClienteRequest>()))
            .Returns(new ValidationResult());

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetClienteByIdQuery>(), CancellationToken.None))
            .ReturnsAsync(savedResponse);

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateClienteCommand>(), CancellationToken.None))
            .ReturnsAsync(updateResponse);

        // Act
        var result = await _controller.Update(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(updateResponse, okResult.Value);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenClienteIsDeleted()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteClienteCommand>(), CancellationToken.None))
            .Returns(Task.FromResult(Unit.Value));

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenClienteDoesNotExist()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteClienteCommand>(), CancellationToken.None))
            .ThrowsAsync(new KeyNotFoundException());

        // Act
        var result = await _controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
