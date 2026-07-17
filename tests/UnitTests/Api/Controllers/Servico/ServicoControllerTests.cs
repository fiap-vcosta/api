using Api.Contracts.Validation;
using Domain.Exceptions;
using Api.Controllers.Servico;
using Api.Controllers.Servico.CreateServico;
using Api.Controllers.Servico.UpdateServico;
using Api.Presenters.Servico;
using Api.ViewModels.Servico;
using Application.UseCases.Administrativo.Servico.Commands;
using Application.UseCases.Administrativo.Servico.Commands.CreateServico;
using Application.UseCases.Administrativo.Servico.Responses;
using Application.UseCases.Administrativo.Servico.Commands.DeleteServico;
using Application.UseCases.Administrativo.Servico.Commands.UpdateServico;
using Application.UseCases.Administrativo.Servico.Queries;
using Application.UseCases.Administrativo.Servico.Queries.GetAllServicos;
using Application.UseCases.Administrativo.Servico.Queries.GetServicoById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Api.Controllers.Servico;

public class ServicoControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IValidator<CreateServicoRequest>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateServicoRequest>> _updateValidatorMock;
    private readonly ServicoController _controller;

    public ServicoControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _createValidatorMock = new Mock<IValidator<CreateServicoRequest>>();
        _updateValidatorMock = new Mock<IValidator<UpdateServicoRequest>>();
        _controller = new ServicoController(
            _mediatorMock.Object,
            new ServicoPresenter(),
            _createValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = new CreateServicoRequest { Codigo = "", Nome = "", PrecoPadrao = 0, Ativo = false };
        var validationResult = new ValidationResult();
        validationResult.Errors.Add("Código inválido");

        _createValidatorMock.Setup(v => v.Validate(It.IsAny<CreateServicoRequest>())).Returns(validationResult);

        // Act
        var result = await _controller.Create(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateServicoRequest { Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true };
        var response = new ServicoResponse { Id = 1, Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true };

        _createValidatorMock.Setup(v => v.Validate(It.IsAny<CreateServicoRequest>())).Returns(new ValidationResult());
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateServicoCommand>(), CancellationToken.None)).ReturnsAsync(response);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ServicoController.GetById), createdResult.ActionName);
        var viewModel = Assert.IsType<ServicoViewModel>(createdResult.Value);
        Assert.Equal(response.Id, viewModel.Id);
        Assert.Equal(response.Codigo, viewModel.Codigo);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenServicoExists()
    {
        // Arrange
        var response = new ServicoResponse { Id = 1, Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetServicoByIdQuery>(), CancellationToken.None)).ReturnsAsync(response);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var viewModel = Assert.IsType<ServicoViewModel>(okResult.Value);
        Assert.Equal(response.Id, viewModel.Id);
        Assert.Equal(response.Nome, viewModel.Nome);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenServicoDoesNotExist()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetServicoByIdQuery>(), CancellationToken.None)).ReturnsAsync((ServicoResponse?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithListOfServicos()
    {
        // Arrange
        var servicos = new List<ServicoResponse>
        {
            new() { Id = 1, Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true },
            new() { Id = 2, Codigo = "FRE-001", Nome = "Freio", PrecoPadrao = 250.00m, Ativo = true }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllServicosQuery>(), CancellationToken.None)).ReturnsAsync(servicos);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var viewModels = Assert.IsAssignableFrom<IEnumerable<ServicoViewModel>>(okResult.Value).ToList();
        Assert.Equal(2, viewModels.Count);
        Assert.Equal(servicos[0].Id, viewModels[0].Id);
        Assert.Equal(servicos[1].Nome, viewModels[1].Nome);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenRequestIsValid()
    {
        // Arrange
        var request = new UpdateServicoRequest { Codigo = "FRE-001", Nome = "Freio", PrecoPadrao = 250.00m, Ativo = true };
        var response = new ServicoResponse { Id = 1, Codigo = "FRE-001", Nome = "Freio", PrecoPadrao = 250.00m, Ativo = true };

        _updateValidatorMock.Setup(v => v.Validate(It.IsAny<UpdateServicoRequest>())).Returns(new ValidationResult());
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateServicoCommand>(), CancellationToken.None)).ReturnsAsync(response);

        // Act
        var result = await _controller.Update(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var viewModel = Assert.IsType<ServicoViewModel>(okResult.Value);
        Assert.Equal(response.PrecoPadrao, viewModel.PrecoPadrao);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenServicoIsDeleted()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteServicoCommand>(), CancellationToken.None)).Returns(Task.FromResult(Unit.Value));

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ThrowsDomainNotFoundException_WhenServicoDoesNotExist()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteServicoCommand>(), CancellationToken.None))
            .ThrowsAsync(new DomainNotFoundException("Serviço não encontrado"));

        // Act & Assert
        await Assert.ThrowsAsync<DomainNotFoundException>(() => _controller.Delete(999));
    }
}
