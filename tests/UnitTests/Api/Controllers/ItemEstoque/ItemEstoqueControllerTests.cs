using Api.Contracts.Validation;
using Api.Controllers.ItemEstoque;
using Api.Controllers.ItemEstoque.CreateItemEstoque;
using Api.Controllers.ItemEstoque.RegistrarEntradaEstoque;
using Api.Controllers.ItemEstoque.UpdateItemEstoque;
using Application.Estoque.ItemEstoque.Commands.CreateItemEstoque;
using Application.Estoque.ItemEstoque.Queries.GetItemEstoqueById;
using Domain.Estoque.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Api.Controllers.ItemEstoque;

public class ItemEstoqueControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IValidator<CreateItemEstoqueRequest>> _createValidatorMock;
    private readonly ItemEstoqueController _controller;

    public ItemEstoqueControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _createValidatorMock = new Mock<IValidator<CreateItemEstoqueRequest>>();
        var updateValidatorMock = new Mock<IValidator<UpdateItemEstoqueRequest>>();
        var registrarEntradaMock = new Mock<IValidator<RegistrarEntradaEstoqueRequest>>();
        
        _controller = new ItemEstoqueController(
            _mediatorMock.Object,
            _createValidatorMock.Object,
            updateValidatorMock.Object,
            registrarEntradaMock.Object
        );
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
            Tipo = ItemTipo.Peca,
            Nome = "Filtro de Óleo",
            UnidadeMedida = UnidadeMedida.Unidade,
            PrecoVenda = 55.50m,
            Saldo = 10.000m,
            SaldoReservado = 2.000m
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateItemEstoqueCommand>(), CancellationToken.None))
            .ReturnsAsync(response);

        var request = new CreateItemEstoqueRequest
        {
            Codigo = "ITM-001",
            Tipo = ItemTipo.Peca,
            Nome = "Filtro de Óleo",
            UnidadeMedida = UnidadeMedida.Unidade,
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
