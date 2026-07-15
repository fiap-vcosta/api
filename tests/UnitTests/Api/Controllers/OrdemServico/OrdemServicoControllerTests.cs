using Api.Contracts.Validation;
using Api.Controllers.OrdemServico;
using Api.Controllers.OrdemServico.AdicionarItemServico;
using Api.Controllers.OrdemServico.AprovarServicosParcialmente;
using Api.Controllers.OrdemServico.ConfirmarExecucao;
using Api.Controllers.OrdemServico.CriarOrdemServico;
using Application.Core.OrdemServico.Commands.AdicionarItemOrdemServico;
using Application.Core.OrdemServico.Commands.AprovarOrdemServico;
using Application.Core.OrdemServico.Commands.AprovarServicosParcialmente;
using Application.Core.OrdemServico.Commands.ConfirmarExecucaoOrdemServico;
using Application.Core.OrdemServico.Commands.ConfirmarPagamentoOrdemServico;
using Application.Core.OrdemServico.Commands.CriarOrdemServico;
using Application.Core.OrdemServico.Commands.DescartarOrdemServico;
using Application.Core.OrdemServico.Commands.FinalizarDiagnostico;
using Application.Core.OrdemServico.Commands.RejeitarOrdemServico;
using Application.Core.OrdemServico.Queries.GetOrdemServicoById;
using Application.Core.OrdemServico.Queries.GetTempoMedioAllServicos;
using Domain.OrdemServico.Entities;
using Domain.OrdemServico.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Api.Controllers.OrdemServico;

public class OrdemServicoControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<IValidator<CriarOrdemServicoRequest>> _criarValidatorMock = new();
    private readonly Mock<IValidator<AdicionarItemServicoRequest>> _adicionarValidatorMock = new();
    private readonly Mock<IValidator<AprovarServicosParcialmenteRequest>> _aprovarParcialValidatorMock = new();
    private readonly Mock<IValidator<ConfirmarExecucaoRequest>> _confirmarExecucaoValidatorMock = new();
    private readonly OrdemServicoController _controller;

    public OrdemServicoControllerTests()
    {
        _controller = new OrdemServicoController(
            _mediatorMock.Object,
            _criarValidatorMock.Object,
            _adicionarValidatorMock.Object,
            _aprovarParcialValidatorMock.Object,
            _confirmarExecucaoValidatorMock.Object);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenOrdemDoesNotExist()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetOrdemServicoByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrdemServicoAggregateRoot?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenOrdemExists()
    {
        // Arrange
        var ordem = OrdemServicoAggregateRoot.Criar(
            new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" },
            new VeiculoOrdemServico { Placa = "ABC-1234", Marca = "VW", Modelo = "Gol" });

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetOrdemServicoByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordem);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(ordem, ok.Value);
    }

    [Fact]
    public async Task CriarOrdemServico_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var validation = new ValidationResult();
        validation.Errors.Add("IdVeiculo deve ser um veículo válido.");
        _criarValidatorMock.Setup(v => v.Validate(It.IsAny<CriarOrdemServicoRequest>())).Returns(validation);

        // Act
        var result = await _controller.CriarOrdemServico(new CriarOrdemServicoRequest { IdVeiculo = 0 });

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CriarOrdemServico_ReturnsCreated_WhenRequestIsValid()
    {
        // Arrange
        _criarValidatorMock.Setup(v => v.Validate(It.IsAny<CriarOrdemServicoRequest>())).Returns(new ValidationResult());

        var response = new CriarOrdemServicoCommandResponse
        {
            Id = 1,
            Status = StatusOrdemServico.Recebida,
            RecebidaEm = DateTime.UtcNow,
            Cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" },
            Veiculo = new VeiculoOrdemServico { Placa = "ABC-1234", Marca = "VW", Modelo = "Gol" }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CriarOrdemServicoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.CriarOrdemServico(new CriarOrdemServicoRequest { IdVeiculo = 1 });

        // Assert
        var created = Assert.IsType<CreatedResult>(result);
        Assert.Same(response, created.Value);
    }

    [Fact]
    public async Task DescartarOrdemServico_ReturnsNotFound_WhenKeyNotFound()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DescartarOrdemServicoCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("não encontrada"));

        // Act
        var result = await _controller.DescartarOrdemServico(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task AdicionarItemServico_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var validation = new ValidationResult();
        validation.Errors.Add("IdServico inválido");
        _adicionarValidatorMock.Setup(v => v.Validate(It.IsAny<AdicionarItemServicoRequest>())).Returns(validation);

        // Act
        var result = await _controller.AdicionarItemServico(1, new AdicionarItemServicoRequest());

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task AdicionarItemServico_ReturnsOk_WhenValid()
    {
        // Arrange
        _adicionarValidatorMock.Setup(v => v.Validate(It.IsAny<AdicionarItemServicoRequest>())).Returns(new ValidationResult());

        var response = new AdicionarItemOrdemServicoCommandResponse
        {
            Id = 1,
            Status = StatusOrdemServico.EmDiagnostico,
            ValorTotal = 100m,
            RecebidaEm = DateTime.UtcNow,
            Cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "maria@teste.com" },
            Veiculo = new VeiculoOrdemServico { Placa = "ABC-1234", Marca = "VW", Modelo = "Gol" },
            Itens = []
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<AdicionarItemOrdemServicoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var request = new AdicionarItemServicoRequest
        {
            IdServico = 1,
            ValorCobrado = 100m,
            ItensNecessarios = []
        };

        // Act
        var result = await _controller.AdicionarItemServico(1, request);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task FinalizarDiagnostico_ReturnsOk()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<FinalizarDiagnosticoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FinalizarDiagnosticoCommandResponse
            {
                Id = 1,
                Status = StatusOrdemServico.AguardandoAprovacao,
                ValorTotal = 100m,
                RecebidaEm = DateTime.UtcNow,
                Cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "m@t.com" },
                Veiculo = new VeiculoOrdemServico { Placa = "ABC", Marca = "VW", Modelo = "Gol" },
                Servicos = []
            });

        // Act
        var result = await _controller.AdicionarItemServico(1);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Aprovar_ReturnsOk()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<AprovarOrdemServicoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AprovarOrdemServicoCommandResponse
            {
                Id = 1,
                Status = StatusOrdemServico.ChecandoEstoque,
                ValorTotal = 100m,
                RecebidaEm = DateTime.UtcNow,
                AprovadaEm = DateTime.UtcNow,
                Cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "m@t.com" },
                Veiculo = new VeiculoOrdemServico { Placa = "ABC", Marca = "VW", Modelo = "Gol" },
                Servicos = []
            });

        // Act
        var result = await _controller.AprovarOdemServico(1);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Rejeitar_ReturnsOk()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<RejeitarOrdemServicoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RejeitarOrdemServicoCommandResponse
            {
                Id = 1,
                Status = StatusOrdemServico.EmDiagnostico,
                ValorTotal = 0m,
                RecebidaEm = DateTime.UtcNow,
                EntregueEm = DateTime.UtcNow,
                Cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "m@t.com" },
                Veiculo = new VeiculoOrdemServico { Placa = "ABC", Marca = "VW", Modelo = "Gol" },
                Servicos = []
            });

        // Act
        var result = await _controller.RejeitarOrdemServico(1);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ConfirmarPagamento_ReturnsOk()
    {
        // Arrange
        var ordem = OrdemServicoAggregateRoot.Criar(
            new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "m@t.com" },
            new VeiculoOrdemServico { Placa = "ABC", Marca = "VW", Modelo = "Gol" });
        typeof(OrdemServicoAggregateRoot).GetProperty(nameof(OrdemServicoAggregateRoot.Status))!
            .SetValue(ordem, StatusOrdemServico.Entregue);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ConfirmarPagamentoOrdemServicoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordem);

        // Act
        var result = await _controller.ConfirmarPagamento(1);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetTempoMediaExecucao_ReturnsOk()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetTempoMedioExecucaoAllServicosQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _controller.GetTempoMediaExecucao();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task AprovarServicosParcialmente_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var validation = new ValidationResult();
        validation.Errors.Add("erro");
        _aprovarParcialValidatorMock.Setup(v => v.Validate(It.IsAny<AprovarServicosParcialmenteRequest>())).Returns(validation);

        // Act
        var result = await _controller.AprovarServicosParcialmente(1, new AprovarServicosParcialmenteRequest());

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task AprovarServicosParcialmente_ReturnsOk_WhenValid()
    {
        // Arrange
        _aprovarParcialValidatorMock.Setup(v => v.Validate(It.IsAny<AprovarServicosParcialmenteRequest>())).Returns(new ValidationResult());
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<AprovarServicosParcialmenteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AprovarServicosParcialmenteCommandResponse
            {
                Id = 1,
                Status = StatusOrdemServico.EmDiagnostico,
                ValorTotal = 100m,
                RecebidaEm = DateTime.UtcNow,
                Cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "m@t.com" },
                Veiculo = new VeiculoOrdemServico { Placa = "ABC", Marca = "VW", Modelo = "Gol" },
                Servicos = []
            });

        // Act
        var result = await _controller.AprovarServicosParcialmente(1, new AprovarServicosParcialmenteRequest { IdsServicosAprovados = [1] });

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task AprovarServicosParcialmente_ReturnsProblem_WhenMediatorThrows()
    {
        // Arrange
        _aprovarParcialValidatorMock.Setup(v => v.Validate(It.IsAny<AprovarServicosParcialmenteRequest>())).Returns(new ValidationResult());
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<AprovarServicosParcialmenteCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("x"));

        // Act
        var result = await _controller.AprovarServicosParcialmente(1, new AprovarServicosParcialmenteRequest { IdsServicosAprovados = [1] });

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public async Task ConfirmarExecucao_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var validation = new ValidationResult();
        validation.Errors.Add("erro");
        _confirmarExecucaoValidatorMock.Setup(v => v.Validate(It.IsAny<ConfirmarExecucaoRequest>())).Returns(validation);

        // Act
        var result = await _controller.ConfirmarExecucao(1, new ConfirmarExecucaoRequest());

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ConfirmarExecucao_ReturnsOk_WhenValid()
    {
        // Arrange
        _confirmarExecucaoValidatorMock.Setup(v => v.Validate(It.IsAny<ConfirmarExecucaoRequest>())).Returns(new ValidationResult());
        var ordem = OrdemServicoAggregateRoot.Criar(
            new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "m@t.com" },
            new VeiculoOrdemServico { Placa = "ABC", Marca = "VW", Modelo = "Gol" });
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ConfirmarExecucaoOrdemServicoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordem);

        // Act
        var result = await _controller.ConfirmarExecucao(1, new ConfirmarExecucaoRequest
        {
            ServicosExecutados = [new ServicoExecutado { IdServico = 1, IniciadoEm = DateTime.UtcNow.AddHours(-1), FinalizadoEm = DateTime.UtcNow }]
        });

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ConfirmarExecucao_ReturnsProblem_WhenMediatorThrows()
    {
        // Arrange
        _confirmarExecucaoValidatorMock.Setup(v => v.Validate(It.IsAny<ConfirmarExecucaoRequest>())).Returns(new ValidationResult());
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ConfirmarExecucaoOrdemServicoCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("x"));

        // Act
        var result = await _controller.ConfirmarExecucao(1, new ConfirmarExecucaoRequest
        {
            ServicosExecutados = [new ServicoExecutado { IdServico = 1, IniciadoEm = DateTime.UtcNow.AddHours(-1), FinalizadoEm = DateTime.UtcNow }]
        });

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public async Task Descartar_ReturnsOk_WhenValid()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DescartarOrdemServicoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DescartarOrdemServicoResponse
            {
                Id = 1,
                Status = StatusOrdemServico.Descartada,
                RecebidaEm = DateTime.UtcNow,
                DescartadaEm = DateTime.UtcNow,
                Itens = [],
                Cliente = new ClienteOrdemServico { Id = 1, Nome = "Maria", Email = "m@t.com" },
                Veiculo = new VeiculoOrdemServico { Placa = "ABC", Marca = "VW", Modelo = "Gol" }
            });

        // Act
        var result = await _controller.DescartarOrdemServico(1);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task FinalizarDiagnostico_ReturnsProblem_WhenMediatorThrows()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<FinalizarDiagnosticoCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("x"));

        // Act
        var result = await _controller.AdicionarItemServico(1);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public async Task GetTempoMediaExecucao_ReturnsProblem_WhenMediatorThrows()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetTempoMedioExecucaoAllServicosQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("x"));

        // Act
        var result = await _controller.GetTempoMediaExecucao();

        // Assert
        Assert.IsType<ObjectResult>(result);
    }
}
