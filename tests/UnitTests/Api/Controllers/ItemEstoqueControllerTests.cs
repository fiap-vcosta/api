using Api.Contracts;
using Api.Controllers.ItemEstoque;
using Application.ItemEstoque.Commands;
using Application.ItemEstoque.Queries;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Moq;

namespace UnitTests.Api.Controllers;

public class ItemEstoqueControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IValidator<CreateItemEstoqueRequest>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateItemEstoqueRequest>> _updateValidatorMock;
    private readonly ItemEstoqueController _controller;

    public ItemEstoqueControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _createValidatorMock = new Mock<IValidator<CreateItemEstoqueRequest>>();
        _updateValidatorMock = new Mock<IValidator<UpdateItemEstoqueRequest>>();
        _controller = new ItemEstoqueController(_mediatorMock.Object, _createValidatorMock.Object, _updateValidatorMock.Object);
    }

    [Fact]
    public async Task Create_ReturnsCreatedResult_WhenRequestIsValid()
    {
        _createValidatorMock.Setup(v => v.Validate(It.IsAny<CreateItemEstoqueRequest>()))
            .Returns(new ValidationResult());

        var response = new ItemEstoqueResponse
        {
            Id = 1,
            Codigo = "ITM-001",
            Tipo = Domain.Entities.ItemTipo.Peca,
            Nome = "Filtro de Óleo",
            UnidadeMedida = Domain.Entities.UnidadeMedida.Unidade,
            PrecoVenda = 55.50m,
            Saldo = 10.000m,
            SaldoReservado = 2.000m
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateItemEstoqueCommand>(), CancellationToken.None))
            .ReturnsAsync(response);

        var request = new CreateItemEstoqueRequest
        {
            Codigo = "ITM-001",
            Tipo = Domain.Entities.ItemTipo.Peca,
            Nome = "Filtro de Óleo",
            UnidadeMedida = Domain.Entities.UnidadeMedida.Unidade,
            PrecoVenda = 55.50m,
            Saldo = 10.000m,
            SaldoReservado = 2.000m
        };

        var result = await _controller.Create(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(1, ((ItemEstoqueResponse)createdResult.Value!).Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenItemDoesNotExist()
    {
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetItemEstoqueByIdQuery>(), CancellationToken.None))
            .ReturnsAsync((ItemEstoqueResponse?)null);

        var result = await _controller.GetById(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
