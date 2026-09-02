using Api.Controllers.OrdemServico.PublicApi;
using Api.Presenters.OrdemServico;
using Api.ViewModels.OrdemServico;
using Application.UseCases.OrdemServico.Commands.AprovarOrdemServico;
using Application.UseCases.OrdemServico.Commands.AprovarOrdemServicoPorToken;
using Application.UseCases.OrdemServico.Commands.RejeitarOrdemServico;
using Application.UseCases.OrdemServico.Commands.RejeitarOrdemServicoPorToken;
using Application.UseCases.OrdemServico.Responses;
using Domain.OrdemServico.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Api.Controllers.OrdemServico.PublicApi;

public class OrdemServicoPublicControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly OrdemServicoPublicController _controller;

    public OrdemServicoPublicControllerTests()
    {
        _controller = new OrdemServicoPublicController(_mediator.Object, new OrdemServicoPresenter());
    }

    [Fact]
    public async Task Aprovar_ReturnsBadRequest_WhenTokenMissing()
    {
        // Arrange / Act
        var result = await _controller.Aprovar(" ");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Aprovar_ReturnsOk_WhenTokenValid()
    {
        // Arrange
        _mediator
            .Setup(m => m.Send(It.IsAny<AprovarOrdemServicoPorTokenCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AprovarOrdemServicoCommandResponse
            {
                Id = 1,
                Status = StatusOrdemServico.ChecandoEstoque,
                ValorTotal = 100m,
                RecebidaEm = DateTime.UtcNow,
                AprovadaEm = DateTime.UtcNow,
                Cliente = new ClienteOrdemServicoResponse { Id = 1, Nome = "Maria", Email = "maria@teste.com" },
                Veiculo = new VeiculoOrdemServicoResponse { Placa = "ABC1234", Marca = "VW", Modelo = "Gol" },
                Servicos = []
            });

        // Act
        var result = await _controller.Aprovar("token-abc");

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<AprovarOrdemServicoViewModel>(ok.Value);
    }

    [Fact]
    public async Task Rejeitar_ReturnsOk_WhenTokenValid()
    {
        // Arrange
        _mediator
            .Setup(m => m.Send(It.IsAny<RejeitarOrdemServicoPorTokenCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RejeitarOrdemServicoCommandResponse
            {
                Id = 1,
                Status = StatusOrdemServico.EmDiagnostico,
                ValorTotal = 0m,
                RecebidaEm = DateTime.UtcNow,
                Cliente = new ClienteOrdemServicoResponse { Id = 1, Nome = "Maria", Email = "maria@teste.com" },
                Veiculo = new VeiculoOrdemServicoResponse { Placa = "ABC1234", Marca = "VW", Modelo = "Gol" },
                Servicos = []
            });

        // Act
        var result = await _controller.Rejeitar("token-abc");

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
}
