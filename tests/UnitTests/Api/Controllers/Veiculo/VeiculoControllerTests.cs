using Api.Contracts.Validation;
using Api.Controllers.Veiculo;
using Api.Controllers.Veiculo.CreateVeiculo;
using Api.Controllers.Veiculo.UpdateVeiculo;
using Application.Administrativo.Veiculo.Commands;
using Application.Administrativo.Veiculo.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Api.Controllers.Veiculo;

public class VeiculoControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IValidator<CreateVeiculoRequest>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateVeiculoRequest>> _updateValidatorMock;
    private readonly VeiculoController _controller;

    public VeiculoControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _createValidatorMock = new Mock<IValidator<CreateVeiculoRequest>>();
        _updateValidatorMock = new Mock<IValidator<UpdateVeiculoRequest>>();
        _controller = new VeiculoController(_mediatorMock.Object, _createValidatorMock.Object, _updateValidatorMock.Object);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenValidationFails()
    {
        var request = new CreateVeiculoRequest { Placa = "", DonoId = 0, Modelo = "", Marca = "" };
        var validationResult = new ValidationResult();
        validationResult.Errors.Add("Placa inválida");

        _createValidatorMock.Setup(v => v.Validate(It.IsAny<CreateVeiculoRequest>())).Returns(validationResult);

        var result = await _controller.Create(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenRequestIsValid()
    {
        var request = new CreateVeiculoRequest { Placa = "ABC-1D23", DonoId = 1, Modelo = "Gol", Marca = "Volkswagen" };
        var response = new VeiculoResponse { Id = 1, Placa = "ABC-1D23", DonoId = 1, Modelo = "Gol", Marca = "Volkswagen" };

        _createValidatorMock.Setup(v => v.Validate(It.IsAny<CreateVeiculoRequest>())).Returns(new ValidationResult());
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateVeiculoCommand>(), CancellationToken.None)).ReturnsAsync(response);

        var result = await _controller.Create(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(VeiculoController.GetById), createdResult.ActionName);
        Assert.Equal(response, createdResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenVeiculoExists()
    {
        var response = new VeiculoResponse { Id = 1, Placa = "ABC-1D23", DonoId = 1, Modelo = "Gol", Marca = "Volkswagen" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetVeiculoByIdQuery>(), CancellationToken.None)).ReturnsAsync(response);

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenVeiculoDoesNotExist()
    {
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetVeiculoByIdQuery>(), CancellationToken.None)).ReturnsAsync((VeiculoResponse?)null);

        var result = await _controller.GetById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithListOfVeiculos()
    {
        var veiculos = new List<VeiculoResponse>
        {
            new() { Id = 1, Placa = "ABC-1D23", DonoId = 1, Modelo = "Gol", Marca = "Volkswagen" },
            new() { Id = 2, Placa = "DEF-2G34", DonoId = 2, Modelo = "Polo", Marca = "Volkswagen" }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllVeiculosQuery>(), CancellationToken.None)).ReturnsAsync(veiculos);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(veiculos, okResult.Value);
    }

    [Fact]
    public async Task GetByDono_ReturnsOkWithVeiculosOfDono()
    {
        var veiculos = new List<VeiculoResponse>
        {
            new() { Id = 1, Placa = "ABC-1D23", DonoId = 1, Modelo = "Gol", Marca = "Volkswagen" }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetVeiculosByDonoQuery>(), CancellationToken.None)).ReturnsAsync(veiculos);

        var result = await _controller.GetByDono(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(veiculos, okResult.Value);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenRequestIsValid()
    {
        var request = new UpdateVeiculoRequest { Placa = "DEF-2G34", DonoId = 1, Modelo = "Polo", Marca = "Volkswagen" };
        var response = new VeiculoResponse { Id = 1, Placa = "DEF-2G34", DonoId = 1, Modelo = "Polo", Marca = "Volkswagen" };

        _updateValidatorMock.Setup(v => v.Validate(It.IsAny<UpdateVeiculoRequest>())).Returns(new ValidationResult());
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateVeiculoCommand>(), CancellationToken.None)).ReturnsAsync(response);

        var result = await _controller.Update(1, request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenVeiculoIsDeleted()
    {
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteVeiculoCommand>(), CancellationToken.None)).Returns(Task.FromResult(Unit.Value));

        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenVeiculoDoesNotExist()
    {
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteVeiculoCommand>(), CancellationToken.None)).ThrowsAsync(new KeyNotFoundException());

        var result = await _controller.Delete(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
