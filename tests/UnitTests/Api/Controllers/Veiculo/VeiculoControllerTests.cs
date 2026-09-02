using Api.Contracts.Validation;
using Domain.Exceptions;
using Api.Controllers.Veiculo;
using Api.Controllers.Veiculo.CreateVeiculo;
using Api.Controllers.Veiculo.UpdateVeiculo;
using Api.Presenters.Veiculo;
using Api.ViewModels.Veiculo;
using Application.UseCases.Administrativo.Veiculo.Commands.CreateVeiculo;
using Application.UseCases.Administrativo.Veiculo.Responses;
using Application.UseCases.Administrativo.Veiculo.Commands.DeleteVeiculo;
using Application.UseCases.Administrativo.Veiculo.Commands.UpdateVeiculo;
using Application.UseCases.Administrativo.Veiculo.Queries.GetAllVeiculos;
using Application.UseCases.Administrativo.Veiculo.Queries.GetVeiculosByCliente;
using Application.UseCases.Administrativo.Veiculo.Queries.GetVeiculoById;
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
        _controller = new VeiculoController(
            _mediatorMock.Object,
            new VeiculoPresenter(),
            _createValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = new CreateVeiculoRequest { Placa = "", IdCliente = 0, Modelo = "", Marca = "" };
        var validationResult = new ValidationResult();
        validationResult.Errors.Add("Placa inválida");

        _createValidatorMock.Setup(v => v.Validate(It.IsAny<CreateVeiculoRequest>())).Returns(validationResult);

        // Act
        var result = await _controller.Create(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateVeiculoRequest { Placa = "ABC-1D23", IdCliente = 1, Modelo = "Gol", Marca = "Volkswagen" };
        var response = new VeiculoResponse { Id = 1, Placa = "ABC-1D23", IdCliente = 1, Modelo = "Gol", Marca = "Volkswagen" };

        _createValidatorMock.Setup(v => v.Validate(It.IsAny<CreateVeiculoRequest>())).Returns(new ValidationResult());
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateVeiculoCommand>(), CancellationToken.None)).ReturnsAsync(response);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(VeiculoController.GetById), createdResult.ActionName);
        var viewModel = Assert.IsType<VeiculoViewModel>(createdResult.Value);
        Assert.Equal(response.Id, viewModel.Id);
        Assert.Equal(response.Placa, viewModel.Placa);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenVeiculoExists()
    {
        // Arrange
        var response = new VeiculoResponse { Id = 1, Placa = "ABC-1D23", IdCliente = 1, Modelo = "Gol", Marca = "Volkswagen" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetVeiculoByIdQuery>(), CancellationToken.None)).ReturnsAsync(response);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var viewModel = Assert.IsType<VeiculoViewModel>(okResult.Value);
        Assert.Equal(response.Id, viewModel.Id);
        Assert.Equal(response.Marca, viewModel.Marca);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenVeiculoDoesNotExist()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetVeiculoByIdQuery>(), CancellationToken.None)).ReturnsAsync((VeiculoResponse?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithListOfVeiculos()
    {
        // Arrange
        var veiculos = new List<VeiculoResponse>
        {
            new() { Id = 1, Placa = "ABC-1D23", IdCliente = 1, Modelo = "Gol", Marca = "Volkswagen" },
            new() { Id = 2, Placa = "DEF-2G34", IdCliente = 2, Modelo = "Polo", Marca = "Volkswagen" }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllVeiculosQuery>(), CancellationToken.None)).ReturnsAsync(veiculos);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var viewModels = Assert.IsAssignableFrom<IEnumerable<VeiculoViewModel>>(okResult.Value).ToList();
        Assert.Equal(2, viewModels.Count);
        Assert.Equal(veiculos[0].Id, viewModels[0].Id);
        Assert.Equal(veiculos[1].Placa, viewModels[1].Placa);
    }

    [Fact]
    public async Task GetByDono_ReturnsOkWithVeiculosOfDono()
    {
        // Arrange
        var veiculos = new List<VeiculoResponse>
        {
            new() { Id = 1, Placa = "ABC-1D23", IdCliente = 1, Modelo = "Gol", Marca = "Volkswagen" }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetVeiculosByClienteQuery>(), CancellationToken.None)).ReturnsAsync(veiculos);

        // Act
        var result = await _controller.GetByDono(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var viewModels = Assert.IsAssignableFrom<IEnumerable<VeiculoViewModel>>(okResult.Value).ToList();
        Assert.Single(viewModels);
        Assert.Equal(veiculos[0].Id, viewModels[0].Id);
        Assert.Equal(veiculos[0].IdCliente, viewModels[0].IdCliente);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenRequestIsValid()
    {
        // Arrange
        var request = new UpdateVeiculoRequest { Placa = "DEF-2G34", IdCliente = 1, Modelo = "Polo", Marca = "Volkswagen" };
        var response = new VeiculoResponse { Id = 1, Placa = "DEF-2G34", IdCliente = 1, Modelo = "Polo", Marca = "Volkswagen" };

        _updateValidatorMock.Setup(v => v.Validate(It.IsAny<UpdateVeiculoRequest>())).Returns(new ValidationResult());
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateVeiculoCommand>(), CancellationToken.None)).ReturnsAsync(response);

        // Act
        var result = await _controller.Update(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var viewModel = Assert.IsType<VeiculoViewModel>(okResult.Value);
        Assert.Equal(response.Modelo, viewModel.Modelo);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenVeiculoIsDeleted()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteVeiculoCommand>(), CancellationToken.None)).Returns(Task.FromResult(Unit.Value));

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ThrowsDomainNotFoundException_WhenVeiculoDoesNotExist()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteVeiculoCommand>(), CancellationToken.None))
            .ThrowsAsync(new DomainNotFoundException("Veículo não encontrado"));

        // Act & Assert
        await Assert.ThrowsAsync<DomainNotFoundException>(() => _controller.Delete(999));
    }
}
