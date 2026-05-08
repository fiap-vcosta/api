using Api.Contracts.Validation;
using Api.Controllers.Servico;
using Api.Controllers.Servico.CreateServico;
using Api.Controllers.Servico.UpdateServico;
using Application.Administrativo.Servico.Commands;
using Application.Administrativo.Servico.Commands.CreateServico;
using Application.Administrativo.Servico.Commands.DeleteServico;
using Application.Administrativo.Servico.Commands.UpdateServico;
using Application.Administrativo.Servico.Queries;
using Application.Administrativo.Servico.Queries.GetAllServicos;
using Application.Administrativo.Servico.Queries.GetServicoById;
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
        _controller = new ServicoController(_mediatorMock.Object, _createValidatorMock.Object, _updateValidatorMock.Object);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenValidationFails()
    {
        var request = new CreateServicoRequest { Codigo = "", Nome = "", PrecoPadrao = 0, Ativo = false };
        var validationResult = new ValidationResult();
        validationResult.Errors.Add("Código inválido");

        _createValidatorMock.Setup(v => v.Validate(It.IsAny<CreateServicoRequest>())).Returns(validationResult);

        var result = await _controller.Create(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenRequestIsValid()
    {
        var request = new CreateServicoRequest { Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true };
        var response = new ServicoResponse { Id = 1, Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true };

        _createValidatorMock.Setup(v => v.Validate(It.IsAny<CreateServicoRequest>())).Returns(new ValidationResult());
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateServicoCommand>(), CancellationToken.None)).ReturnsAsync(response);

        var result = await _controller.Create(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ServicoController.GetById), createdResult.ActionName);
        Assert.Equal(response, createdResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenServicoExists()
    {
        var response = new ServicoResponse { Id = 1, Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetServicoByIdQuery>(), CancellationToken.None)).ReturnsAsync(response);

        var result = await _controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenServicoDoesNotExist()
    {
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetServicoByIdQuery>(), CancellationToken.None)).ReturnsAsync((ServicoResponse?)null);

        var result = await _controller.GetById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithListOfServicos()
    {
        var servicos = new List<ServicoResponse>
        {
            new() { Id = 1, Codigo = "OLE-001", Nome = "Óleo", PrecoPadrao = 150.00m, Ativo = true },
            new() { Id = 2, Codigo = "FRE-001", Nome = "Freio", PrecoPadrao = 250.00m, Ativo = true }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllServicosQuery>(), CancellationToken.None)).ReturnsAsync(servicos);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(servicos, okResult.Value);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenRequestIsValid()
    {
        var request = new UpdateServicoRequest { Codigo = "FRE-001", Nome = "Freio", PrecoPadrao = 250.00m, Ativo = true };
        var response = new ServicoResponse { Id = 1, Codigo = "FRE-001", Nome = "Freio", PrecoPadrao = 250.00m, Ativo = true };

        _updateValidatorMock.Setup(v => v.Validate(It.IsAny<UpdateServicoRequest>())).Returns(new ValidationResult());
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateServicoCommand>(), CancellationToken.None)).ReturnsAsync(response);

        var result = await _controller.Update(1, request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenServicoIsDeleted()
    {
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteServicoCommand>(), CancellationToken.None)).Returns(Task.FromResult(Unit.Value));

        var result = await _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenServicoDoesNotExist()
    {
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteServicoCommand>(), CancellationToken.None)).ThrowsAsync(new KeyNotFoundException());

        var result = await _controller.Delete(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
